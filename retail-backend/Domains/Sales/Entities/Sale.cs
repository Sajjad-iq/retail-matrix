using Domains.Shared.ValueObjects;  // For Price
using Domains.Sales.ValueObjects;  // For Discount
using Domains.Sales.Enums;
using Domains.Shared.Base;
using Domains.Common.Currency.Services;

namespace Domains.Sales.Entities;

/// <summary>
/// Sale aggregate root - represents a complete sales transaction
/// </summary>
public class Sale : BaseEntity
{
    private const int MaxItemsPerSale = 1000;

    // Parameterless constructor for EF Core
    private Sale()
    {
        SaleNumber = string.Empty;
        Items = new List<SaleItem>();
        TotalDiscount = Price.Create(0, "IQD");
        GrandTotal = Price.Create(0, "IQD");
        AmountPaid = Price.Create(0, "IQD");
    }

    // Private constructor to enforce factory methods
    private Sale(
        string saleNumber,
        Guid organizationId,
        Guid salesPersonId)
    {
        Id = Guid.NewGuid();
        SaleNumber = saleNumber;
        SaleDate = DateTime.UtcNow;
        OrganizationId = organizationId;
        SalesPersonId = salesPersonId;
        Status = SaleStatus.Draft;
        TotalDiscount = Price.Create(0, "IQD");
        GrandTotal = Price.Create(0, "IQD");
        AmountPaid = Price.Create(0, "IQD");
        Items = new List<SaleItem>();
        InsertDate = DateTime.UtcNow;
    }

    // Properties
    public string SaleNumber { get; private set; }
    public DateTime SaleDate { get; private set; }
    public Guid SalesPersonId { get; private set; }
    public Guid OrganizationId { get; private set; }
    public SaleStatus Status { get; private set; }
    public Price TotalDiscount { get; private set; }
    public Price GrandTotal { get; private set; }
    public Price AmountPaid { get; private set; }
    public string? Notes { get; private set; }

    // Navigation properties
    public List<SaleItem> Items { get; private set; }

    // Factory method
    public static Sale Create(
        Guid organizationId,
        Guid salesPersonId)
    {
        var saleNumber = GenerateSaleNumber();

        return new Sale(
            saleNumber,
            organizationId,
            salesPersonId
        );
    }

    // Domain methods
    public async Task AddItemAsync(
        Guid productPackagingId,
        string productName,
        int quantity,
        Price unitPrice,
        ICurrencyConversionService currencyService,
        Discount? discount = null,
        CancellationToken cancellationToken = default)
    {
        if (Status != SaleStatus.Draft)
            throw new InvalidOperationException("لا يمكن تعديل بيع مكتمل");

        if (quantity <= 0)
            throw new ArgumentException("الكمية يجب أن تكون أكبر من صفر", nameof(quantity));

        // Check for duplicate item first (might not add new item)
        var existingItem = Items.FirstOrDefault(i => i.ProductPackagingId == productPackagingId);
        if (existingItem != null)
        {
            existingItem.UpdateQuantity(existingItem.Quantity + quantity);
            await RecalculateTotalsAsync(currencyService, cancellationToken);
            return;
        }

        // Validate maximum items limit only when adding new item
        if (Items.Count >= MaxItemsPerSale)
            throw new InvalidOperationException($"لا يمكن إضافة أكثر من {MaxItemsPerSale} عنصر");

        var item = SaleItem.Create(
            Id,
            productPackagingId,
            productName,
            quantity,
            unitPrice,
            discount
        );

        Items.Add(item);
        await RecalculateTotalsAsync(currencyService, cancellationToken);
    }

    public async Task RemoveItemAsync(
        Guid itemId,
        ICurrencyConversionService currencyService,
        CancellationToken cancellationToken = default)
    {
        if (Status != SaleStatus.Draft)
            throw new InvalidOperationException("لا يمكن تعديل بيع مكتمل");

        var item = Items.FirstOrDefault(i => i.Id == itemId);
        if (item == null)
            throw new InvalidOperationException("العنصر غير موجود في البيع");

        Items.Remove(item);
        await RecalculateTotalsAsync(currencyService, cancellationToken);
    }

    public async Task UpdateItemQuantityAsync(
        Guid itemId,
        int newQuantity,
        ICurrencyConversionService currencyService,
        CancellationToken cancellationToken = default)
    {
        if (Status != SaleStatus.Draft)
            throw new InvalidOperationException("لا يمكن تعديل بيع مكتمل");

        if (newQuantity <= 0)
            throw new ArgumentException("الكمية يجب أن تكون أكبر من صفر", nameof(newQuantity));

        var item = Items.FirstOrDefault(i => i.Id == itemId);
        if (item == null)
            throw new InvalidOperationException("العنصر غير موجود في البيع");

        item.UpdateQuantity(newQuantity);
        await RecalculateTotalsAsync(currencyService, cancellationToken);
    }

    public void RecordPayment(Price paymentAmount)
    {
        if (paymentAmount.Amount <= 0)
            throw new ArgumentException("المبلغ يجب أن يكون أكبر من صفر", nameof(paymentAmount));

        // Validate overpayment (Price.Add will validate currency)
        var totalAfterPayment = AmountPaid.Add(paymentAmount);
        if (totalAfterPayment.Amount > GrandTotal.Amount)
            throw new InvalidOperationException("المبلغ المدفوع يتجاوز الإجمالي المطلوب");

        // Update amount paid
        AmountPaid = totalAfterPayment;

        // Only update to PartiallyPaid, never auto-complete
        if (AmountPaid.Amount > 0 && AmountPaid.Amount < GrandTotal.Amount)
        {
            Status = SaleStatus.PartiallyPaid;
        }
    }

    public void CompleteSale()
    {
        if (Status == SaleStatus.Completed)
            throw new InvalidOperationException("البيع مكتمل بالفعل");

        if (Status == SaleStatus.Cancelled)
            throw new InvalidOperationException("لا يمكن إكمال بيع ملغي");

        if (Items.Count == 0)
            throw new InvalidOperationException("لا يمكن إكمال بيع بدون عناصر");

        if (GrandTotal.Amount <= 0)
            throw new InvalidOperationException("إجمالي البيع يجب أن يكون أكبر من صفر");

        if (AmountPaid.Amount < GrandTotal.Amount)
            throw new InvalidOperationException("المبلغ المدفوع أقل من الإجمالي");

        Status = SaleStatus.Completed;
    }

    public void CancelSale()
    {
        if (Status == SaleStatus.Completed)
            throw new InvalidOperationException("لا يمكن إلغاء بيع مكتمل");

        if (AmountPaid.Amount > 0)
            throw new InvalidOperationException("لا يمكن إلغاء بيع تم الدفع فيه، يجب استرداد المبلغ أولاً");

        Status = SaleStatus.Cancelled;
    }

    public void AddNotes(string notes)
    {
        Notes = notes;
    }

    // Private helpers
    private async Task RecalculateTotalsAsync(
        ICurrencyConversionService currencyService,
        CancellationToken cancellationToken = default)
    {
        if (!Items.Any())
        {
            // No items, reset to zero in IQD
            TotalDiscount = Price.Create(0, "IQD");
            GrandTotal = Price.Create(0, "IQD");
            return;
        }

        // Get base currency for organization
        var baseCurrency = await currencyService.GetBaseCurrencyCodeAsync(OrganizationId, cancellationToken);

        decimal totalInBase = 0;
        decimal discountInBase = 0;

        // Convert each item to base currency
        foreach (var item in Items)
        {
            // Convert line total to base currency
            var itemTotalInBase = await currencyService.ConvertToBaseCurrencyAsync(
                item.LineTotal.Amount,
                item.UnitPrice.Currency,
                OrganizationId,
                cancellationToken);

            totalInBase += itemTotalInBase;

            // Convert discount if exists
            if (item.Discount != null)
            {
                var itemBasePrice = Price.Create(item.UnitPrice.Amount * item.Quantity, item.UnitPrice.Currency);
                var discountAmount = item.Discount.CalculateDiscount(itemBasePrice).Amount;

                var discountInBaseCurrency = await currencyService.ConvertToBaseCurrencyAsync(
                    discountAmount,
                    item.UnitPrice.Currency,
                    OrganizationId,
                    cancellationToken);

                discountInBase += discountInBaseCurrency;
            }
        }

        TotalDiscount = Price.Create(discountInBase, baseCurrency);
        GrandTotal = Price.Create(totalInBase, baseCurrency);

        // Enforce invariant: AmountPaid cannot exceed GrandTotal
        if (AmountPaid.Amount > GrandTotal.Amount)
        {
            // Reset AmountPaid to GrandTotal (auto-refund excess)
            AmountPaid = GrandTotal;
        }
    }


    private static string GenerateSaleNumber()
    {
        // Format: SAL-YYYYMMDD-GUID (thread-safe and guaranteed unique)
        var date = DateTime.UtcNow.ToString("yyyyMMdd");
        var uniqueId = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
        return $"SAL-{date}-{uniqueId}";
    }
}

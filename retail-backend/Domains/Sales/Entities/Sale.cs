using Domains.Shared.ValueObjects;  // For Price
using Domains.Sales.ValueObjects;  // For Discount
using Domains.Sales.Enums;
using Domains.Shared.Base;

namespace Domains.Sales.Entities;

/// <summary>
/// Sale aggregate root - represents a complete sales transaction
/// </summary>
public class Sale : BaseEntity
{
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
        var saleNumber = GenerateSaleNumber(organizationId);

        return new Sale(
            saleNumber,
            organizationId,
            salesPersonId
        );
    }

    // Domain methods
    public void AddItem(
        Guid productPackagingId,
        string productName,
        int quantity,
        Price unitPrice,
        Discount? discount = null)
    {
        if (Status != SaleStatus.Draft)
            throw new InvalidOperationException("لا يمكن تعديل بيع مكتمل");

        if (quantity <= 0)
            throw new ArgumentException("الكمية يجب أن تكون أكبر من صفر", nameof(quantity));

        var item = SaleItem.Create(
            Id,
            productPackagingId,
            productName,
            quantity,
            unitPrice,
            discount
        );

        Items.Add(item);
        RecalculateTotals();
    }

    public void RemoveItem(Guid itemId)
    {
        if (Status != SaleStatus.Draft)
            throw new InvalidOperationException("لا يمكن تعديل بيع مكتمل");

        var item = Items.FirstOrDefault(i => i.Id == itemId);
        if (item != null)
        {
            Items.Remove(item);
            RecalculateTotals();
        }
    }

    public void UpdateItemQuantity(Guid itemId, int newQuantity)
    {
        if (Status != SaleStatus.Draft)
            throw new InvalidOperationException("لا يمكن تعديل بيع مكتمل");

        if (newQuantity <= 0)
            throw new ArgumentException("الكمية يجب أن تكون أكبر من صفر", nameof(newQuantity));

        var item = Items.FirstOrDefault(i => i.Id == itemId);
        if (item != null)
        {
            item.UpdateQuantity(newQuantity);
            RecalculateTotals();
        }
    }

    public void RecordPayment(Price paymentAmount)
    {
        if (paymentAmount.Amount <= 0)
            throw new ArgumentException("المبلغ يجب أن يكون أكبر من صفر", nameof(paymentAmount));

        // Validate overpayment
        var totalAfterPayment = AmountPaid.Add(paymentAmount);
        if (totalAfterPayment.Amount > GrandTotal.Amount)
            throw new InvalidOperationException("المبلغ المدفوع يتجاوز الإجمالي المطلوب");

        // Update amount paid
        AmountPaid = totalAfterPayment;

        // Update sale status
        UpdateStatusBasedOnPayment();
    }

    public void CompleteSale()
    {
        if (Status != SaleStatus.Draft && Status != SaleStatus.PartiallyPaid)
            throw new InvalidOperationException("البيع مكتمل بالفعل");

        if (Items.Count == 0)
            throw new InvalidOperationException("لا يمكن إكمال بيع بدون عناصر");

        if (AmountPaid.Amount < GrandTotal.Amount)
            throw new InvalidOperationException("المبلغ المدفوع أقل من الإجمالي");

        Status = SaleStatus.Completed;
    }

    public void CancelSale()
    {
        if (Status == SaleStatus.Completed)
            throw new InvalidOperationException("لا يمكن إلغاء بيع مكتمل");

        Status = SaleStatus.Cancelled;
    }

    public void AddNotes(string notes)
    {
        Notes = notes;
    }

    // Private helpers
    private void RecalculateTotals()
    {
        // Calculate grand total from all line items
        var itemsTotal = Items.Sum(i => i.LineTotal.Amount);

        // Calculate total discount from all items
        var totalDiscount = Items
            .Where(i => i.Discount != null)
            .Sum(i => i.Discount!.CalculateDiscount(
                Price.Create(i.UnitPrice.Amount * i.Quantity, i.UnitPrice.Currency)
            ).Amount);

        TotalDiscount = Price.Create(totalDiscount, "IQD");
        GrandTotal = Price.Create(itemsTotal, "IQD");
    }

    private void UpdateStatusBasedOnPayment()
    {
        // Only update to PartiallyPaid, never auto-complete
        if (AmountPaid.Amount > 0 && AmountPaid.Amount < GrandTotal.Amount)
        {
            Status = SaleStatus.PartiallyPaid;
        }
    }

    private static string GenerateSaleNumber(Guid organizationId)
    {
        // Format: SAL-YYYYMMDD-XXXXX
        var date = DateTime.UtcNow.ToString("yyyyMMdd");
        var random = new Random().Next(10000, 99999);
        return $"SAL-{date}-{random}";
    }
}

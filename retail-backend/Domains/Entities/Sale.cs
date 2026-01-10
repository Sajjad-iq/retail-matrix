using Domains.Enums;
using Domains.Shared;
using Domains.ValueObjects;

namespace Domains.Entities;

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
        Payments = new List<Payment>();
        TotalDiscount = Price.Create(0, "IQD");
        GrandTotal = Price.Create(0, "IQD");
        AmountPaid = Price.Create(0, "IQD");
    }

    // Private constructor to enforce factory methods
    private Sale(
        string saleNumber,
        Guid organizationId,
        Guid salesPersonId,
        Guid? customerId,
        string? customerName,
        Guid? locationId)
    {
        Id = Guid.NewGuid();
        SaleNumber = saleNumber;
        SaleDate = DateTime.UtcNow;
        OrganizationId = organizationId;
        SalesPersonId = salesPersonId;
        CustomerId = customerId;
        CustomerName = customerName;
        LocationId = locationId;
        Status = SaleStatus.Draft;
        TotalDiscount = Price.Create(0, "IQD");
        GrandTotal = Price.Create(0, "IQD");
        AmountPaid = Price.Create(0, "IQD");
        Items = new List<SaleItem>();
        Payments = new List<Payment>();
        InsertDate = DateTime.UtcNow;
    }

    // Properties
    public string SaleNumber { get; private set; }
    public DateTime SaleDate { get; private set; }
    public Guid? CustomerId { get; private set; }
    public string? CustomerName { get; private set; }
    public Guid SalesPersonId { get; private set; }
    public Guid OrganizationId { get; private set; }
    public Guid? LocationId { get; private set; }
    public SaleStatus Status { get; private set; }
    public Price TotalDiscount { get; private set; }
    public Price GrandTotal { get; private set; }
    public Price AmountPaid { get; private set; }
    public string? Notes { get; private set; }

    // Navigation properties
    public List<SaleItem> Items { get; private set; }
    public List<Payment> Payments { get; private set; }

    // Factory method
    public static Sale Create(
        Guid organizationId,
        Guid salesPersonId,
        Guid? customerId = null,
        string? customerName = null,
        Guid? locationId = null)
    {
        var saleNumber = GenerateSaleNumber(organizationId);

        return new Sale(
            saleNumber,
            organizationId,
            salesPersonId,
            customerId,
            customerName,
            locationId
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

    public void AddPayment(PaymentMethod method, Price amount, string? referenceNumber = null)
    {
        if (amount.Amount <= 0)
            throw new ArgumentException("المبلغ يجب أن يكون أكبر من صفر", nameof(amount));

        var payment = Payment.Create(Id, method, amount, referenceNumber);
        Payments.Add(payment);

        // Update amount paid
        AmountPaid = Price.Create(
            Payments.Where(p => p.Status == PaymentStatus.Completed).Sum(p => p.Amount.Amount),
            "IQD"
        );

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
        UpdateDate = DateTime.UtcNow;
    }

    public void CancelSale()
    {
        if (Status == SaleStatus.Completed)
            throw new InvalidOperationException("لا يمكن إلغاء بيع مكتمل");

        Status = SaleStatus.Cancelled;
        UpdateDate = DateTime.UtcNow;
    }

    public void AddNotes(string notes)
    {
        Notes = notes;
        UpdateDate = DateTime.UtcNow;
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

        UpdateDate = DateTime.UtcNow;
    }

    private void UpdateStatusBasedOnPayment()
    {
        if (AmountPaid.Amount >= GrandTotal.Amount && Status == SaleStatus.Draft)
        {
            Status = SaleStatus.Completed;
        }
        else if (AmountPaid.Amount > 0 && AmountPaid.Amount < GrandTotal.Amount)
        {
            Status = SaleStatus.PartiallyPaid;
        }

        UpdateDate = DateTime.UtcNow;
    }

    private static string GenerateSaleNumber(Guid organizationId)
    {
        // Format: SAL-YYYYMMDD-XXXXX
        var date = DateTime.UtcNow.ToString("yyyyMMdd");
        var random = new Random().Next(10000, 99999);
        return $"SAL-{date}-{random}";
    }
}

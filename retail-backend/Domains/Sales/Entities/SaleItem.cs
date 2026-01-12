using Domains.Shared.ValueObjects;  // For Price
using Domains.Sales.ValueObjects;  // For Discount
using Domains.Shared.Base;

namespace Domains.Sales.Entities;

/// <summary>
/// Sale item entity - represents a line item in a sale
/// </summary>
public class SaleItem : BaseEntity
{
    // Parameterless constructor for EF Core
    private SaleItem()
    {
        ProductName = string.Empty;
        UnitPrice = Price.Create(0, "IQD");
        LineTotal = Price.Create(0, "IQD");
    }

    // Private constructor to enforce factory methods
    private SaleItem(
        Guid saleId,
        Guid productPackagingId,
        string productName,
        int quantity,
        Price unitPrice,
        Discount? discount)
    {
        Id = Guid.NewGuid();
        SaleId = saleId;
        ProductPackagingId = productPackagingId;
        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;
        Discount = discount;
        CalculateLineTotal();
        InsertDate = DateTime.UtcNow;
    }

    // Properties
    public Guid SaleId { get; private set; }
    public Guid ProductPackagingId { get; private set; }
    public string ProductName { get; private set; }
    public int Quantity { get; private set; }
    public Price UnitPrice { get; private set; }
    public Discount? Discount { get; private set; }
    public Price LineTotal { get; private set; }

    // Factory method
    public static SaleItem Create(
        Guid saleId,
        Guid productPackagingId,
        string productName,
        int quantity,
        Price unitPrice,
        Discount? discount = null)
    {
        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("اسم المنتج مطلوب", nameof(productName));

        if (quantity <= 0)
            throw new ArgumentException("الكمية يجب أن تكون أكبر من صفر", nameof(quantity));

        return new SaleItem(
            saleId,
            productPackagingId,
            productName,
            quantity,
            unitPrice,
            discount
        );
    }

    // Domain methods
    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new ArgumentException("الكمية يجب أن تكون أكبر من صفر", nameof(newQuantity));

        Quantity = newQuantity;
        CalculateLineTotal();
    }

    public void UpdateDiscount(Discount? discount)
    {
        Discount = discount;
        CalculateLineTotal();
    }

    public void UpdatePrice(Price newPrice)
    {
        if (newPrice.Amount <= 0)
            throw new ArgumentException("السعر يجب أن يكون أكبر من صفر", nameof(newPrice));

        UnitPrice = newPrice;
        CalculateLineTotal();
    }

    public void CalculateLineTotal()
    {
        var baseAmount = Price.Create(UnitPrice.Amount * Quantity, UnitPrice.Currency);

        if (Discount != null)
        {
            var discountAmount = Discount.CalculateDiscount(baseAmount);
            LineTotal = baseAmount.Subtract(discountAmount);
        }
        else
        {
            LineTotal = baseAmount;
        }
    }
}

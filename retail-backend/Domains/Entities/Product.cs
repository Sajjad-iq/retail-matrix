using Domains.Enums;
using Domains.Shared;
using Domains.ValueObjects;

namespace Domains.Entities;

/// <summary>
/// Product aggregate root representing an inventory item
/// </summary>
public class Product : BaseEntity
{
    // Parameterless constructor for EF Core
    private Product()
    {
        Name = string.Empty;
        Description = string.Empty;
        CostPrice = Price.Create(0, "IQD");
        SellingPrice = Price.Create(0, "IQD");
    }

    // Private constructor to enforce factory methods
    private Product(
        string name,
        Price costPrice,
        Price sellingPrice,
        UnitOfMeasure unitOfMeasure,
        Guid organizationId,
        string? description = null,
        string? barcode = null,
        string? imageUrl = null,
        int reorderLevel = 10)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description ?? string.Empty;
        Barcode = barcode;
        CostPrice = costPrice;
        SellingPrice = sellingPrice;
        UnitOfMeasure = unitOfMeasure;
        CurrentStock = 0;
        ReorderLevel = reorderLevel;
        OrganizationId = organizationId;
        ImageUrl = imageUrl;
        Status = ProductStatus.Active;
        InsertDate = DateTime.UtcNow;
        UpdateDate = DateTime.UtcNow;
    }

    // Properties with proper encapsulation
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string? Barcode { get; private set; } // should move to packaging
    public Price CostPrice { get; private set; } // should move to packaging
    public Price SellingPrice { get; private set; } // should move to packaging
    public UnitOfMeasure UnitOfMeasure { get; private set; } // should move to packaging
    public int CurrentStock { get; private set; } // should move to stock
    public int ReorderLevel { get; private set; } // should move to packaging
    public ProductStatus Status { get; private set; }
    public Guid OrganizationId { get; private set; }
    public string? ImageUrl { get; private set; }

    /// <summary>
    /// Factory method to create a new product
    /// </summary>
    public static Product Create(
        string name,
        Price costPrice,
        Price sellingPrice,
        UnitOfMeasure unitOfMeasure,
        Guid organizationId,
        string? description = null,
        string? barcode = null,
        string? imageUrl = null,
        int reorderLevel = 10)
    {
        // Validate using value objects
        var productName = ValueObjects.Name.Create(name, minLength: 2, maxLength: 200);

        // Validate prices - ensure same currency
        if (costPrice.Currency != sellingPrice.Currency)
            throw new ArgumentException("سعر التكلفة وسعر البيع يجب أن يكونا بنفس العملة");

        // Validate Barcode if provided
        if (barcode != null)
        {
            _ = ValueObjects.Barcode.Create(barcode); // Validate only
        }

        if (organizationId == Guid.Empty)
            throw new ArgumentException("معرف المؤسسة مطلوب", nameof(organizationId));

        if (sellingPrice.Amount < costPrice.Amount)
            throw new ArgumentException("سعر البيع يجب أن يكون أكبر من أو يساوي سعر التكلفة", nameof(sellingPrice));

        if (reorderLevel < 0)
            throw new ArgumentException("مستوى إعادة الطلب يجب أن يكون صفر أو أكثر", nameof(reorderLevel));

        return new Product(
            name: productName,
            costPrice: costPrice,
            sellingPrice: sellingPrice,
            unitOfMeasure: unitOfMeasure,
            organizationId: organizationId,
            description: description?.Trim(),
            barcode: barcode,
            imageUrl: imageUrl,
            reorderLevel: reorderLevel
        );
    }

    // Domain Methods
    public void UpdateBasicInfo(
        string name,
        string? description = null,
        string? imageUrl = null)
    {
        var productName = ValueObjects.Name.Create(name, minLength: 2, maxLength: 200);

        Name = productName;
        Description = description?.Trim() ?? Description;
        ImageUrl = imageUrl ?? ImageUrl;
        UpdateDate = DateTime.UtcNow;
    }

    public void UpdatePricing(Price costPrice, Price sellingPrice)
    {
        // Validate prices - ensure same currency
        if (costPrice.Currency != sellingPrice.Currency)
            throw new ArgumentException("سعر التكلفة وسعر البيع يجب أن يكونا بنفس العملة");

        if (sellingPrice.Amount < costPrice.Amount)
            throw new ArgumentException("سعر البيع يجب أن يكون أكبر من أو يساوي سعر التكلفة", nameof(sellingPrice));

        CostPrice = costPrice;
        SellingPrice = sellingPrice;
        UpdateDate = DateTime.UtcNow;
    }

    public void UpdateBarcode(string barcode)
    {
        var productBarcode = ValueObjects.Barcode.Create(barcode);
        Barcode = productBarcode;
        UpdateDate = DateTime.UtcNow;
    }

    public void UpdateReorderLevel(int reorderLevel)
    {
        if (reorderLevel < 0)
            throw new ArgumentException("مستوى إعادة الطلب يجب أن يكون صفر أو أكثر", nameof(reorderLevel));

        ReorderLevel = reorderLevel;
        UpdateDate = DateTime.UtcNow;
    }

    public void AdjustStock(int quantity, string reason)
    {
        var newStock = CurrentStock + quantity;

        if (newStock < 0)
            throw new InvalidOperationException("لا يمكن أن يكون المخزون سالب");

        CurrentStock = newStock;
        UpdateStockStatus();
        UpdateDate = DateTime.UtcNow;
    }

    public void SetStock(int quantity)
    {
        if (quantity < 0)
            throw new ArgumentException("الكمية لا يمكن أن تكون سالبة", nameof(quantity));

        CurrentStock = quantity;
        UpdateStockStatus();
        UpdateDate = DateTime.UtcNow;
    }

    private void UpdateStockStatus()
    {
        if (CurrentStock == 0 && Status == ProductStatus.Active)
        {
            Status = ProductStatus.OutOfStock;
        }
        else if (CurrentStock > 0 && Status == ProductStatus.OutOfStock)
        {
            Status = ProductStatus.Active;
        }
    }

    public void Activate()
    {
        if (Status == ProductStatus.Discontinued)
            throw new InvalidOperationException("لا يمكن تفعيل منتج متوقف");

        Status = ProductStatus.Active;
        UpdateDate = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        Status = ProductStatus.Inactive;
        UpdateDate = DateTime.UtcNow;
    }

    public void Discontinue()
    {
        Status = ProductStatus.Discontinued;
        UpdateDate = DateTime.UtcNow;
    }

    public bool IsLowStock() => CurrentStock > 0 && CurrentStock <= ReorderLevel;

    public bool IsOutOfStock() => CurrentStock == 0;

    public decimal GetProfitMargin() => SellingPrice.Subtract(CostPrice).Amount;

    public decimal GetProfitMarginPercentage() => CostPrice.Amount > 0 ? ((SellingPrice.Amount - CostPrice.Amount) / CostPrice.Amount) * 100 : 0;
}

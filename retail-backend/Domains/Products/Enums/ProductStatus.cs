namespace Domains.Products.Enums;

/// <summary>
/// Defines the status of a product in the inventory
/// </summary>
public enum ProductStatus
{
    /// <summary>
    /// Product is active and available for sale
    /// </summary>
    Active,

    /// <summary>
    /// Product is inactive/disabled
    /// </summary>
    Inactive,

    /// <summary>
    /// Product is out of stock
    /// </summary>
    OutOfStock,

    /// <summary>
    /// Product is discontinued
    /// </summary>
    Discontinued
}

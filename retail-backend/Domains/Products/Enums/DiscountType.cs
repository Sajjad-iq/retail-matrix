namespace Domains.Products.Enums;

/// <summary>
/// Represents the type of discount applied to a product packaging
/// </summary>
public enum DiscountType
{
    /// <summary>
    /// No discount applied
    /// </summary>
    None,

    /// <summary>
    /// Discount as a percentage of the selling price (e.g., 10% off)
    /// </summary>
    Percentage,

    /// <summary>
    /// Discount as a fixed amount subtracted from the selling price (e.g., 5000 IQD off)
    /// </summary>
    FixedAmount
}

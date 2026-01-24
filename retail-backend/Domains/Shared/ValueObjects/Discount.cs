using System.Text.Json.Serialization;
using Domains.Products.Enums;

namespace Domains.Shared.ValueObjects;

/// <summary>
/// Value object representing a discount that can be either a percentage or a fixed amount
/// </summary>
public sealed class Discount : IEquatable<Discount>
{
    public decimal Value { get; }
    public DiscountType Type { get; }

    // Parameterless constructor for EF Core
    private Discount()
    {
        Value = 0;
        Type = DiscountType.None;
    }

    [JsonConstructor]
    public Discount(decimal value, DiscountType type)
    {
        Value = value;
        Type = type;
    }

    /// <summary>
    /// Creates a new discount
    /// </summary>
    /// <param name="value">The discount value (percentage 0-100 or fixed amount)</param>
    /// <param name="type">The type of discount</param>
    public static Discount Create(decimal value, DiscountType type)
    {
        if (type == DiscountType.None)
        {
            return new Discount(0, DiscountType.None);
        }

        if (value < 0)
            throw new ArgumentException("قيمة الخصم لا يمكن أن تكون سالبة", nameof(value));

        if (type == DiscountType.Percentage && value > 100)
            throw new ArgumentException("نسبة الخصم يجب أن تكون بين 0 و 100", nameof(value));

        return new Discount(value, type);
    }

    /// <summary>
    /// Creates a percentage-based discount
    /// </summary>
    /// <param name="percentage">The discount percentage (0-100)</param>
    public static Discount Percentage(decimal percentage)
    {
        return Create(percentage, DiscountType.Percentage);
    }

    /// <summary>
    /// Creates a fixed amount discount
    /// </summary>
    /// <param name="amount">The fixed discount amount</param>
    public static Discount FixedAmount(decimal amount)
    {
        return Create(amount, DiscountType.FixedAmount);
    }

    /// <summary>
    /// Creates a zero/no discount
    /// </summary>
    public static Discount None() => new Discount(0, DiscountType.None);

    /// <summary>
    /// Calculates the discounted price based on the original price
    /// </summary>
    /// <param name="originalPrice">The original price before discount</param>
    /// <returns>The price after applying the discount</returns>
    public Price ApplyTo(Price originalPrice)
    {
        if (Type == DiscountType.None || Value == 0)
            return originalPrice;

        var discountedAmount = Type switch
        {
            DiscountType.Percentage => originalPrice.Amount * (1 - Value / 100),
            DiscountType.FixedAmount => Math.Max(0, originalPrice.Amount - Value),
            _ => originalPrice.Amount
        };

        return Price.Create(discountedAmount, originalPrice.Currency);
    }

    /// <summary>
    /// Calculates the discount amount for a given price (returns the amount saved)
    /// </summary>
    /// <param name="originalPrice">The original price</param>
    /// <returns>The discount amount as a Price</returns>
    public Price CalculateDiscount(Price originalPrice)
    {
        if (Type == DiscountType.None || Value == 0)
            return Price.Create(0, originalPrice.Currency);

        var discountAmount = Type switch
        {
            DiscountType.Percentage => originalPrice.Amount * (Value / 100),
            DiscountType.FixedAmount => Math.Min(Value, originalPrice.Amount),
            _ => 0
        };

        return Price.Create(discountAmount, originalPrice.Currency);
    }

    /// <summary>
    /// Gets the discount amount for a given price as a decimal
    /// </summary>
    /// <param name="originalPrice">The original price</param>
    /// <returns>The discount amount</returns>
    public decimal GetDiscountAmount(Price originalPrice)
    {
        return CalculateDiscount(originalPrice).Amount;
    }

    /// <summary>
    /// Checks if the discount has any value
    /// </summary>
    public bool HasDiscount => Type != DiscountType.None && Value > 0;

    public bool Equals(Discount? other)
    {
        if (other is null) return false;
        return Value == other.Value && Type == other.Type;
    }

    public override bool Equals(object? obj) => Equals(obj as Discount);

    public override int GetHashCode() => HashCode.Combine(Value, Type);

    public static bool operator ==(Discount? left, Discount? right)
    {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    public static bool operator !=(Discount? left, Discount? right)
    {
        return !(left == right);
    }

    public override string ToString() => Type switch
    {
        DiscountType.None => "لا يوجد خصم",
        DiscountType.Percentage => $"{Value:N2}%",
        DiscountType.FixedAmount => $"{Value:N2}",
        _ => "غير معروف"
    };
}

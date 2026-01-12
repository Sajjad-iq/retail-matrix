using Domains.Products.Enums;
using Domains.Shared.ValueObjects;

namespace Domains.Sales.ValueObjects;

/// <summary>
/// Value object representing a discount (percentage or fixed amount)
/// </summary>
public class Discount
{
    public decimal Value { get; }
    public DiscountType Type { get; }

    // Parameterless constructor for EF Core
    private Discount()
    {
        Value = 0;
        Type = DiscountType.FixedAmount;
    }

    private Discount(decimal value, DiscountType type)
    {
        if (value < 0)
            throw new ArgumentException("الخصم لا يمكن أن يكون سالباً", nameof(value));

        Value = value;
        Type = type;
    }

    public static Discount Create(decimal value, DiscountType type)
    {
        if (value < 0)
            throw new ArgumentException("الخصم لا يمكن أن يكون سالباً", nameof(value));

        if (type == DiscountType.Percentage && value > 100)
            throw new ArgumentException("نسبة الخصم لا يمكن أن تتجاوز 100%", nameof(value));

        return new Discount(value, type);
    }

    public static Discount None() => new Discount(0, DiscountType.FixedAmount);

    public Price CalculateDiscount(Price originalPrice)
    {
        if (Type == DiscountType.Percentage)
        {
            var discountAmount = originalPrice.Amount * (Value / 100);
            return Price.Create(discountAmount, originalPrice.Currency);
        }
        else
        {
            // Cap fixed discount at original price to prevent negative totals
            var discountAmount = Math.Min(Value, originalPrice.Amount);
            return Price.Create(discountAmount, originalPrice.Currency);
        }
    }

    // Equality
    public override bool Equals(object? obj)
    {
        if (obj is not Discount other)
            return false;

        return Value == other.Value && Type == other.Type;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Value, Type);
    }

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
}

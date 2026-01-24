using System.Text.Json.Serialization;
using Domains.Products.Enums;

namespace Domains.Shared.ValueObjects;

/// <summary>
/// Value object representing a weight measurement
/// </summary>
public sealed class Weight : IEquatable<Weight>
{
    public decimal Value { get; }
    public UnitOfMeasure Unit { get; }

    [JsonConstructor]
    public Weight(decimal value, UnitOfMeasure unit)
    {
        if (value < 0)
            throw new ArgumentException("الوزن لا يمكن أن يكون سالب", nameof(value));

        if (!IsValidWeightUnit(unit))
            throw new ArgumentException($"وحدة القياس {unit} غير صالحة للوزن", nameof(unit));

        Value = value;
        Unit = unit;
    }

    public static Weight Create(decimal value, UnitOfMeasure unit = UnitOfMeasure.Kilogram)
    {
        return new Weight(value, unit);
    }

    private static bool IsValidWeightUnit(UnitOfMeasure unit)
    {
        return unit is UnitOfMeasure.Kilogram
            or UnitOfMeasure.Gram
            or UnitOfMeasure.Milligram
            or UnitOfMeasure.MetricTon
            or UnitOfMeasure.Pound
            or UnitOfMeasure.Ounce;
    }

    public Weight ConvertTo(UnitOfMeasure targetUnit)
    {
        if (!IsValidWeightUnit(targetUnit))
            throw new ArgumentException($"وحدة القياس {targetUnit} غير صالحة للوزن", nameof(targetUnit));

        if (Unit == targetUnit)
            return this;

        // Convert to kilograms first (base unit)
        var valueInKg = Unit switch
        {
            UnitOfMeasure.Kilogram => Value,
            UnitOfMeasure.Gram => Value / 1000m,
            UnitOfMeasure.Milligram => Value / 1_000_000m,
            UnitOfMeasure.MetricTon => Value * 1000m,
            UnitOfMeasure.Pound => Value * 0.453592m,
            UnitOfMeasure.Ounce => Value * 0.0283495m,
            _ => throw new InvalidOperationException($"تحويل الوحدة {Unit} غير مدعوم")
        };

        // Convert from kilograms to target unit
        var result = targetUnit switch
        {
            UnitOfMeasure.Kilogram => valueInKg,
            UnitOfMeasure.Gram => valueInKg * 1000m,
            UnitOfMeasure.Milligram => valueInKg * 1_000_000m,
            UnitOfMeasure.MetricTon => valueInKg / 1000m,
            UnitOfMeasure.Pound => valueInKg / 0.453592m,
            UnitOfMeasure.Ounce => valueInKg / 0.0283495m,
            _ => throw new InvalidOperationException($"تحويل الوحدة {targetUnit} غير مدعوم")
        };

        return new Weight(result, targetUnit);
    }

    public Weight Add(Weight other)
    {
        var otherConverted = other.ConvertTo(Unit);
        return new Weight(Value + otherConverted.Value, Unit);
    }

    public Weight Subtract(Weight other)
    {
        var otherConverted = other.ConvertTo(Unit);
        var result = Value - otherConverted.Value;

        if (result < 0)
            throw new InvalidOperationException("النتيجة لا يمكن أن تكون سالبة");

        return new Weight(result, Unit);
    }

    public bool Equals(Weight? other)
    {
        if (other is null) return false;

        // Compare by converting both to kilograms for accurate comparison
        var thisInKg = ConvertTo(UnitOfMeasure.Kilogram).Value;
        var otherInKg = other.ConvertTo(UnitOfMeasure.Kilogram).Value;

        return Math.Abs(thisInKg - otherInKg) < 0.000001m; // Tolerance for decimal comparison
    }

    public override bool Equals(object? obj) => Equals(obj as Weight);

    public override int GetHashCode() => HashCode.Combine(ConvertTo(UnitOfMeasure.Kilogram).Value);

    public override string ToString() => $"{Value:N2} {GetUnitAbbreviation()}";

    private string GetUnitAbbreviation() => Unit switch
    {
        UnitOfMeasure.Kilogram => "kg",
        UnitOfMeasure.Gram => "g",
        UnitOfMeasure.Milligram => "mg",
        UnitOfMeasure.MetricTon => "t",
        UnitOfMeasure.Pound => "lb",
        UnitOfMeasure.Ounce => "oz",
        _ => Unit.ToString()
    };
}

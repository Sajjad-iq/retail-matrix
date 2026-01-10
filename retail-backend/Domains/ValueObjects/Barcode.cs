using System.Text.RegularExpressions;

namespace Domains.ValueObjects;

/// <summary>
/// Value object representing a product barcode
/// </summary>
public sealed class Barcode : IEquatable<Barcode>
{
    private static readonly Regex BarcodeRegex = new(@"^\d{8,13}$", RegexOptions.Compiled);

    public string Value { get; }

    private Barcode(string value)
    {
        Value = value;
    }

    public static Barcode Create(string barcode)
    {
        if (string.IsNullOrWhiteSpace(barcode))
            throw new ArgumentException("الباركود مطلوب", nameof(barcode));

        var normalizedBarcode = barcode.Trim();

        if (!BarcodeRegex.IsMatch(normalizedBarcode))
            throw new ArgumentException("الباركود يجب أن يحتوي على 8-13 رقم فقط", nameof(barcode));

        return new Barcode(normalizedBarcode);
    }

    public bool Equals(Barcode? other)
    {
        if (other is null) return false;
        return Value.Equals(other.Value, StringComparison.Ordinal);
    }

    public override bool Equals(object? obj) => Equals(obj as Barcode);

    public override int GetHashCode() => Value.GetHashCode(StringComparison.Ordinal);

    public override string ToString() => Value;

    public static implicit operator string(Barcode barcode) => barcode.Value;
}

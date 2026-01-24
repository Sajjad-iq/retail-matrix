using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Domains.Shared.ValueObjects;

/// <summary>
/// Value object representing a product barcode
/// </summary>
public sealed class Barcode : IEquatable<Barcode>
{
    private static readonly Regex BarcodeRegex = new(@"^\d{8,13}$", RegexOptions.Compiled);

    public string Value { get; }

    [JsonConstructor]
    public Barcode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("الباركود مطلوب", nameof(value));

        var normalizedBarcode = value.Trim();

        if (!BarcodeRegex.IsMatch(normalizedBarcode))
            throw new ArgumentException("الباركود يجب أن يحتوي على 8-13 رقم فقط", nameof(value));

        Value = normalizedBarcode;
    }

    public static Barcode Create(string barcode)
    {
        return new Barcode(barcode);
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

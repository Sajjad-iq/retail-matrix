using System.Text.RegularExpressions;

namespace Domains.Common.Currency.ValueObjects;

/// <summary>
/// Value object representing a currency code (ISO 4217)
/// </summary>
public sealed class CurrencyCode : IEquatable<CurrencyCode>
{
    private static readonly Regex CodeRegex = new(@"^[A-Z]{3}$", RegexOptions.Compiled);

    public string Value { get; }

    private CurrencyCode(string value)
    {
        Value = value;
    }

    public static CurrencyCode Create(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("رمز العملة مطلوب", nameof(code));

        var normalizedCode = code.Trim().ToUpperInvariant();

        if (!CodeRegex.IsMatch(normalizedCode))
            throw new ArgumentException("رمز العملة يجب أن يكون 3 أحرف كبيرة (مثل IQD, USD, EUR)", nameof(code));

        return new CurrencyCode(normalizedCode);
    }

    public bool Equals(CurrencyCode? other)
    {
        if (other is null) return false;
        return Value.Equals(other.Value, StringComparison.Ordinal);
    }

    public override bool Equals(object? obj) => Equals(obj as CurrencyCode);

    public override int GetHashCode() => Value.GetHashCode(StringComparison.Ordinal);

    public override string ToString() => Value;

    public static implicit operator string(CurrencyCode code) => code.Value;
}

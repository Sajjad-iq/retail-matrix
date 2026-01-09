using System.Text.RegularExpressions;

namespace Domains.ValueObjects;

/// <summary>
/// Value object representing a domain name (e.g., example.com) with validation
/// </summary>
public sealed class DomainName : IEquatable<DomainName>
{
    private static readonly Regex DomainRegex = new(
        @"^(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?\.)+[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase
    );

    public string Value { get; }

    private DomainName(string value)
    {
        Value = value;
    }

    public static DomainName Create(string domain)
    {
        if (string.IsNullOrWhiteSpace(domain))
            throw new ArgumentException("النطاق مطلوب", nameof(domain));

        var normalizedDomain = domain.Trim().ToLowerInvariant();

        if (!DomainRegex.IsMatch(normalizedDomain))
            throw new ArgumentException("صيغة النطاق غير صحيحة", nameof(domain));

        if (normalizedDomain.Length > 253)
            throw new ArgumentException("النطاق يجب ألا يتجاوز 253 حرف", nameof(domain));

        return new DomainName(normalizedDomain);
    }

    public bool Equals(DomainName? other)
    {
        if (other is null) return false;
        return Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj) => Equals(obj as DomainName);

    public override int GetHashCode() => Value.GetHashCode(StringComparison.OrdinalIgnoreCase);

    public override string ToString() => Value;

    public static implicit operator string(DomainName domain) => domain.Value;
}

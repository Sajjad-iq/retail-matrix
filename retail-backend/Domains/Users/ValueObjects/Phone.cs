using System.Text.RegularExpressions;

namespace Domains.Users.ValueObjects;

/// <summary>
/// Value object representing a phone number with validation
/// </summary>
public sealed class Phone : IEquatable<Phone>
{
    private static readonly Regex PhoneRegex = new(
        @"^\+?[1-9]\d{1,14}$",
        RegexOptions.Compiled
    );

    public string Value { get; }

    private Phone(string value)
    {
        Value = value;
    }

    public static Phone Create(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("رقم الهاتف مطلوب", nameof(phoneNumber));

        // Remove spaces, dashes, and parentheses
        var normalized = Regex.Replace(phoneNumber, @"[\s\-\(\)]", "");

        if (!PhoneRegex.IsMatch(normalized))
            throw new ArgumentException("صيغة رقم الهاتف غير صحيحة", nameof(phoneNumber));

        return new Phone(normalized);
    }

    public bool Equals(Phone? other)
    {
        if (other is null) return false;
        return Value.Equals(other.Value);
    }

    public override bool Equals(object? obj) => Equals(obj as Phone);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;

    public static implicit operator string(Phone phone) => phone.Value;
}

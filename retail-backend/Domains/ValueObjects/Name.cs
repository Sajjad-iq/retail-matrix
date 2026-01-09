namespace Domains.ValueObjects;

/// <summary>
/// Value object representing a person or organization name with validation
/// </summary>
public sealed class Name : IEquatable<Name>
{
    public string Value { get; }

    private Name(string value)
    {
        Value = value;
    }

    public static Name Create(string name, int minLength = 2, int maxLength = 200)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("الاسم مطلوب", nameof(name));

        var trimmedName = name.Trim();

        if (trimmedName.Length < minLength)
            throw new ArgumentException($"الاسم يجب أن يكون {minLength} أحرف على الأقل", nameof(name));

        if (trimmedName.Length > maxLength)
            throw new ArgumentException($"الاسم يجب ألا يتجاوز {maxLength} حرف", nameof(name));

        return new Name(trimmedName);
    }

    public bool Equals(Name? other)
    {
        if (other is null) return false;
        return Value.Equals(other.Value, StringComparison.Ordinal);
    }

    public override bool Equals(object? obj) => Equals(obj as Name);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;

    public static implicit operator string(Name name) => name.Value;
}

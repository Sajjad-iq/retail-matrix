namespace Domains.Users.ValueObjects;

/// <summary>
/// Value object representing an email address with validation
/// </summary>
public sealed class Email : IEquatable<Email>
{
    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("البريد الإلكتروني مطلوب", nameof(email));

        var normalizedEmail = email.Trim().ToLowerInvariant();

        if (!IsValidEmail(normalizedEmail))
            throw new ArgumentException("صيغة البريد الإلكتروني غير صحيحة", nameof(email));

        if (normalizedEmail.Length > 254)
            throw new ArgumentException("البريد الإلكتروني يجب ألا يتجاوز 254 حرف", nameof(email));

        return new Email(normalizedEmail);
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    public bool Equals(Email? other)
    {
        if (other is null) return false;
        return Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj) => Equals(obj as Email);

    public override int GetHashCode() => Value.GetHashCode(StringComparison.OrdinalIgnoreCase);

    public override string ToString() => Value;

    public static implicit operator string(Email email) => email.Value;
}

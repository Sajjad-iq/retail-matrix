using System.Text.RegularExpressions;

namespace Domains.Users.ValueObjects;

/// <summary>
/// Value object representing a password with validation rules
/// </summary>
public sealed class Password : IEquatable<Password>
{
    private static readonly Regex UppercaseRegex = new(@"[A-Z]", RegexOptions.Compiled);
    private static readonly Regex LowercaseRegex = new(@"[a-z]", RegexOptions.Compiled);
    private static readonly Regex DigitRegex = new(@"\d", RegexOptions.Compiled);
    private static readonly Regex SpecialCharRegex = new(@"[!@#$%^&*(),.?""':{}|<>]", RegexOptions.Compiled);

    public string Value { get; }

    private Password(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a password with basic validation (minimum 8 characters)
    /// </summary>
    public static Password Create(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("كلمة المرور مطلوبة", nameof(password));

        if (password.Length < 8)
            throw new ArgumentException("كلمة المرور يجب أن تكون 8 أحرف على الأقل", nameof(password));

        return new Password(password);
    }

    /// <summary>
    /// Creates a password with strong validation rules
    /// </summary>
    public static Password CreateStrong(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("كلمة المرور مطلوبة", nameof(password));

        if (password.Length < 8)
            throw new ArgumentException("كلمة المرور يجب أن تكون 8 أحرف على الأقل", nameof(password));

        if (password.Length > 128)
            throw new ArgumentException("كلمة المرور يجب ألا تتجاوز 128 حرف", nameof(password));

        if (!UppercaseRegex.IsMatch(password))
            throw new ArgumentException("كلمة المرور يجب أن تحتوي على حرف كبير واحد على الأقل", nameof(password));

        if (!LowercaseRegex.IsMatch(password))
            throw new ArgumentException("كلمة المرور يجب أن تحتوي على حرف صغير واحد على الأقل", nameof(password));

        if (!DigitRegex.IsMatch(password))
            throw new ArgumentException("كلمة المرور يجب أن تحتوي على رقم واحد على الأقل", nameof(password));

        if (!SpecialCharRegex.IsMatch(password))
            throw new ArgumentException("كلمة المرور يجب أن تحتوي على رمز خاص واحد على الأقل", nameof(password));

        return new Password(password);
    }

    /// <summary>
    /// Validates password strength without creating an instance
    /// </summary>
    public static bool IsStrong(string password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 8 || password.Length > 128)
            return false;

        return UppercaseRegex.IsMatch(password) &&
               LowercaseRegex.IsMatch(password) &&
               DigitRegex.IsMatch(password) &&
               SpecialCharRegex.IsMatch(password);
    }

    public bool Equals(Password? other)
    {
        if (other is null) return false;
        return Value.Equals(other.Value);
    }

    public override bool Equals(object? obj) => Equals(obj as Password);

    public override int GetHashCode() => Value.GetHashCode();


    public static implicit operator string(Password password) => password.Value;
}

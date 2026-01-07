using Domains.Services;

namespace Infrastructure.Security;

/// <summary>
/// BCrypt implementation of password hashing
/// Uses BCrypt.Net library with work factor of 12
/// </summary>
public class BCryptPasswordHasher : IPasswordHasher
{
    private const int WorkFactor = 12;

    /// <summary>
    /// Hashes a password using BCrypt with automatic salt generation
    /// </summary>
    /// <param name="password">Plain text password</param>
    /// <returns>BCrypt hash (60 characters)</returns>
    /// <exception cref="ArgumentException">Thrown when password is null or empty</exception>
    public string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("كلمة المرور مطلوبة", nameof(password));
        }

        return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
    }

    /// <summary>
    /// Verifies a password against a BCrypt hash
    /// </summary>
    /// <param name="password">Plain text password to verify</param>
    /// <param name="hashedPassword">BCrypt hash to verify against</param>
    /// <returns>True if password matches, false otherwise</returns>
    /// <exception cref="ArgumentException">Thrown when password or hash is null or empty</exception>
    public bool VerifyPassword(string password, string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("كلمة المرور مطلوبة", nameof(password));
        }

        if (string.IsNullOrWhiteSpace(hashedPassword))
        {
            throw new ArgumentException("كلمة المرور المشفرة مطلوبة", nameof(hashedPassword));
        }

        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
        catch
        {
            // Invalid hash format
            return false;
        }
    }
}

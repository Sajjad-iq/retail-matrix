namespace Domains.Shared.ValueObjects;

/// <summary>
/// Value object representing a monetary price
/// </summary>
public sealed class Price : IEquatable<Price>
{
    public decimal Amount { get; }
    public string Currency { get; }

    // Parameterless constructor for EF Core
    private Price()
    {
        Amount = 0;
        Currency = "IQD";
    }

    private Price(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Price Create(decimal amount, string currency = "IQD")
    {
        if (amount < 0)
            throw new ArgumentException("السعر لا يمكن أن يكون سالب", nameof(amount));

        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("العملة مطلوبة", nameof(currency));

        var normalizedCurrency = currency.Trim().ToUpperInvariant();

        if (normalizedCurrency.Length != 3)
            throw new ArgumentException("رمز العملة يجب أن يكون 3 أحرف (مثل IQD, USD)", nameof(currency));

        return new Price(amount, normalizedCurrency);
    }

    public Price Add(Price other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException($"لا يمكن جمع أسعار بعملات مختلفة: {Currency} و {other.Currency}");

        return new Price(Amount + other.Amount, Currency);
    }

    public Price Subtract(Price other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException($"لا يمكن طرح أسعار بعملات مختلفة: {Currency} و {other.Currency}");

        var result = Amount - other.Amount;
        if (result < 0)
            throw new InvalidOperationException("النتيجة لا يمكن أن تكون سالبة");

        return new Price(result, Currency);
    }

    public Price Multiply(decimal factor)
    {
        if (factor < 0)
            throw new ArgumentException("المضاعف لا يمكن أن يكون سالب", nameof(factor));

        return new Price(Amount * factor, Currency);
    }

    public bool Equals(Price? other)
    {
        if (other is null) return false;
        return Amount == other.Amount && Currency.Equals(other.Currency, StringComparison.Ordinal);
    }

    public override bool Equals(object? obj) => Equals(obj as Price);

    public override int GetHashCode() => HashCode.Combine(Amount, Currency);

    public override string ToString() => $"{Amount:N2} {Currency}";
}

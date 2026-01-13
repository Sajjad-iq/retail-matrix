namespace Domains.Common.Currency.Services;

/// <summary>
/// Domain service for currency conversion operations
/// </summary>
public interface ICurrencyConversionService
{
    /// <summary>
    /// Convert amount from one currency to organization's base currency
    /// </summary>
    Task<decimal> ConvertToBaseCurrencyAsync(
        decimal amount,
        string fromCurrency,
        Guid organizationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get organization's base currency code
    /// </summary>
    Task<string> GetBaseCurrencyCodeAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Convert amount between two currencies
    /// </summary>
    Task<decimal> ConvertAsync(
        decimal amount,
        string fromCurrency,
        string toCurrency,
        Guid organizationId,
        CancellationToken cancellationToken = default);
}

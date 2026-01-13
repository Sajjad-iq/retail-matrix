using Domains.Common.Currency.Repositories;
using Domains.Common.Currency.Services;
using Microsoft.Extensions.Caching.Memory;
using Currency = Domains.Common.Currency.Entities.Currency;

namespace Infrastructure.Services;

/// <summary>
/// Implementation of currency conversion service with caching
/// </summary>
public class CurrencyConversionService : ICurrencyConversionService
{
    private readonly ICurrencyRepository _currencyRepository;
    private readonly IMemoryCache _cache;
    private const int CacheDurationMinutes = 60;

    public CurrencyConversionService(
        ICurrencyRepository currencyRepository,
        IMemoryCache cache)
    {
        _currencyRepository = currencyRepository;
        _cache = cache;
    }

    public async Task<decimal> ConvertToBaseCurrencyAsync(
        decimal amount,
        string fromCurrency,
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        // If already in base currency, return as is
        var baseCurrency = await GetBaseCurrencyAsync(organizationId, cancellationToken);
        if (fromCurrency.Equals(baseCurrency.Code, StringComparison.OrdinalIgnoreCase))
            return amount;

        // Get source currency
        var sourceCurrency = await GetCurrencyByCodeAsync(fromCurrency, organizationId, cancellationToken);
        if (sourceCurrency == null)
            throw new InvalidOperationException($"العملة {fromCurrency} غير موجودة");

        // Convert to base currency
        return sourceCurrency.ConvertToBaseCurrency(amount);
    }

    public async Task<string> GetBaseCurrencyCodeAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        var baseCurrency = await GetBaseCurrencyAsync(organizationId, cancellationToken);
        return baseCurrency.Code;
    }

    public async Task<decimal> ConvertAsync(
        decimal amount,
        string fromCurrency,
        string toCurrency,
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        // If same currency, return as is
        if (fromCurrency.Equals(toCurrency, StringComparison.OrdinalIgnoreCase))
            return amount;

        // Convert to base currency first
        var amountInBase = await ConvertToBaseCurrencyAsync(amount, fromCurrency, organizationId, cancellationToken);

        // If target is base currency, we're done
        var baseCurrency = await GetBaseCurrencyAsync(organizationId, cancellationToken);
        if (toCurrency.Equals(baseCurrency.Code, StringComparison.OrdinalIgnoreCase))
            return amountInBase;

        // Convert from base to target currency
        var targetCurrency = await GetCurrencyByCodeAsync(toCurrency, organizationId, cancellationToken);
        if (targetCurrency == null)
            throw new InvalidOperationException($"العملة {toCurrency} غير موجودة");

        return targetCurrency.ConvertFromBaseCurrency(amountInBase);
    }

    // Private helper methods with caching
    private async Task<Currency> GetBaseCurrencyAsync(
        Guid organizationId,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"base_currency_{organizationId}";

        if (!_cache.TryGetValue(cacheKey, out Currency? baseCurrency))
        {
            baseCurrency = await _currencyRepository.GetBaseCurrencyAsync(organizationId, cancellationToken);
            if (baseCurrency == null)
                throw new InvalidOperationException("العملة الأساسية غير موجودة للمؤسسة");

            _cache.Set(cacheKey, baseCurrency, TimeSpan.FromMinutes(CacheDurationMinutes));
        }

        return baseCurrency!;
    }

    private async Task<Currency?> GetCurrencyByCodeAsync(
        string code,
        Guid organizationId,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"currency_{organizationId}_{code}";

        if (!_cache.TryGetValue(cacheKey, out Currency? currency))
        {
            currency = await _currencyRepository.GetByCodeAsync(code, organizationId, cancellationToken);
            if (currency != null)
            {
                _cache.Set(cacheKey, currency, TimeSpan.FromMinutes(CacheDurationMinutes));
            }
        }

        return currency;
    }
}

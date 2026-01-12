using Domains.Common.Currency.Enums;
using Domains.Common.Currency.ValueObjects;
using Domains.Shared.Base;

namespace Domains.Common.Currency.Entities;

/// <summary>
/// Currency entity - represents a currency with exchange rate
/// </summary>
public class Currency : BaseEntity
{
    // Parameterless constructor for EF Core
    private Currency()
    {
        Code = string.Empty;
        Name = string.Empty;
        Symbol = string.Empty;
    }

    // Private constructor to enforce factory methods
    private Currency(
        string code,
        string name,
        string symbol,
        decimal exchangeRate,
        bool isBaseCurrency,
        Guid organizationId)
    {
        Id = Guid.NewGuid();
        Code = code;
        Name = name;
        Symbol = symbol;
        ExchangeRate = exchangeRate;
        IsBaseCurrency = isBaseCurrency;
        Status = CurrencyStatus.Active;
        OrganizationId = organizationId;
        InsertDate = DateTime.UtcNow;
    }

    // Properties
    public string Code { get; private set; }
    public string Name { get; private set; }
    public string Symbol { get; private set; }
    public decimal ExchangeRate { get; private set; }
    public bool IsBaseCurrency { get; private set; }
    public CurrencyStatus Status { get; private set; }
    public Guid OrganizationId { get; private set; }

    /// <summary>
    /// Factory method to create a new currency
    /// </summary>
    public static Currency Create(
        string code,
        string name,
        string symbol,
        decimal exchangeRate,
        bool isBaseCurrency,
        Guid organizationId)
    {
        // Validate currency code
        var currencyCode = CurrencyCode.Create(code);

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("اسم العملة مطلوب", nameof(name));

        if (string.IsNullOrWhiteSpace(symbol))
            throw new ArgumentException("رمز العملة مطلوب", nameof(symbol));

        if (organizationId == Guid.Empty)
            throw new ArgumentException("معرف المؤسسة مطلوب", nameof(organizationId));

        // Validate exchange rate based on currency type
        if (isBaseCurrency)
        {
            if (exchangeRate != 1.0m)
                throw new ArgumentException("سعر الصرف للعملة الأساسية يجب أن يكون 1.0", nameof(exchangeRate));
        }
        else
        {
            if (exchangeRate <= 0)
                throw new ArgumentException("سعر الصرف يجب أن يكون أكبر من صفر", nameof(exchangeRate));
        }

        return new Currency(
            code: currencyCode,
            name: name.Trim(),
            symbol: symbol.Trim(),
            exchangeRate: exchangeRate,
            isBaseCurrency: isBaseCurrency,
            organizationId: organizationId
        );
    }

    // Domain Methods
    public void UpdateExchangeRate(decimal newRate)
    {
        if (IsBaseCurrency)
            throw new InvalidOperationException("لا يمكن تغيير سعر صرف العملة الأساسية");

        if (newRate <= 0)
            throw new ArgumentException("سعر الصرف يجب أن يكون أكبر من صفر", nameof(newRate));

        ExchangeRate = newRate;
    }

    public void UpdateInfo(string name, string symbol)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("اسم العملة مطلوب", nameof(name));

        if (string.IsNullOrWhiteSpace(symbol))
            throw new ArgumentException("رمز العملة مطلوب", nameof(symbol));

        Name = name.Trim();
        Symbol = symbol.Trim();
    }

    public void Activate()
    {
        Status = CurrencyStatus.Active;
    }

    public void Deactivate()
    {
        if (IsBaseCurrency)
            throw new InvalidOperationException("لا يمكن تعطيل العملة الأساسية");

        Status = CurrencyStatus.Inactive;
    }

    public decimal ConvertToBaseCurrency(decimal amount)
    {
        if (IsBaseCurrency)
            return amount;

        return amount * ExchangeRate;
    }

    public decimal ConvertFromBaseCurrency(decimal baseAmount)
    {
        if (IsBaseCurrency)
            return baseAmount;

        if (ExchangeRate == 0)
            throw new InvalidOperationException("سعر الصرف لا يمكن أن يكون صفر");

        return baseAmount / ExchangeRate;
    }
}

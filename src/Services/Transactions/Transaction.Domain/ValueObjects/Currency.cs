namespace Transactions.Domain.ValueObjects;

public sealed record Currency
{
    internal static readonly Currency None = new("");
    public static readonly Currency Usd = new("USD");
    public static readonly Currency Eur = new("EUR");
    public static readonly Currency EGP = new("EGP");
    public string Code { get; }
    private Currency(string code)
    {
        Code = code;
    }
    public static Currency Of(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ValidationException([new ValidationError(nameof(Currency), "Currency code is required.")]);
        return new Currency(code.ToUpper());
    }

    /// <summary>
    /// Parses a currency code from persistence; returns None for null or empty.
    /// </summary>
    public static Currency FromCode(string? code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return None;
        return new Currency(code.ToUpper());
    }
}
public static class AllowedCurrencies
{
    public static IReadOnlyList<Currency> All { get; } = new List<Currency>
    {
        Currency.Usd,
        Currency.Eur,
        Currency.EGP
    }.AsReadOnly();
    public static bool IsValid(string code)
    {
        return All.Any(c => c.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
    }

}

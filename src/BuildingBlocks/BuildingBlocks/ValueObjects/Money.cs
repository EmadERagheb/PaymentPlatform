namespace BuildingBlocks.ValueObjects;

public class Money
{
    public decimal Amount { get; }
    public Currency Currency { get; }

    public Money(decimal amount, Currency currency)
    {
        if (amount < 0)
            throw new ValidationException([new ValidationError(nameof(Amount), "Amount cannot be negative.")]);
        Amount = amount;
        Currency = currency;
    }
    public static Money Zero() => new(0, Currency.None);
    public static Money Zero(Currency currency) => new(0, currency);
    public bool IsZero() => this == Zero(Currency);
}

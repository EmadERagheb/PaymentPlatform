namespace BuildingBlocks.ValueObjects;

public sealed record TransactionId
{
    public Guid Value { get; }
    private TransactionId(Guid value)
    {
        Value = value;
    }
    public static TransactionId Of(Guid value)
    {
        if (value == Guid.Empty)
            throw new ValidationException([new ValidationError(nameof(TransactionId), "TrasactionId cannot be empty.")]);
        return new TransactionId(value);
    }
}

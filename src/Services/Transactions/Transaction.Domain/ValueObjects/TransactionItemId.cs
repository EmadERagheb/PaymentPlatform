using Newtonsoft.Json;

namespace Transactions.Domain.ValueObjects;

public sealed record TransactionItemId
{
    public Guid Value { get; }
    [JsonConstructor]
    private TransactionItemId(Guid value)
    {
        Value = value;
    }
    public static TransactionItemId Of(Guid value)
    {
        if (value == Guid.Empty)
            throw new ValidationException([new ValidationError(nameof(TransactionItemId),"TransactionItemId cannot be empty.")]);
        return new TransactionItemId(value);
    }
}

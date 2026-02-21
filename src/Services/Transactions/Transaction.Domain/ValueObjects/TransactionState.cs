namespace Transactions.Domain.ValueObjects;

public enum TransactionState
{
    Draft = 0,
    Submitted = 1,
    Completed = 2,
    Cancelled = 3
}

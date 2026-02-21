namespace Transactions.Application.Transactions.Commands.CreateTransaction;

public sealed record CreatedTransactionResult(Guid TransactionId, string Currency);

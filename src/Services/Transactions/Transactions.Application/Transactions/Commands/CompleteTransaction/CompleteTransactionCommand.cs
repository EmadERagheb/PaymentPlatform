namespace Transactions.Application.Transactions.Commands.CompleteTransaction;

public sealed record CompleteTransactionCommand(Guid TransactionId) : ICommand<Result>;

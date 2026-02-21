namespace Transactions.Application.Transactions.Commands.CancelTransaction;

public sealed record CancelTransactionCommand(Guid TransactionId) : ICommand<Result>;


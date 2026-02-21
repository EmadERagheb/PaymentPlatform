namespace Transactions.Application.Transactions.Commands.SubmitTransaction;

public sealed record SubmitTransactionCommand(Guid TransactionId) : ICommand<Result>;


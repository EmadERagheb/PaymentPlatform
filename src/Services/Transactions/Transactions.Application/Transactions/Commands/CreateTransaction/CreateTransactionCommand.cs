namespace Transactions.Application.Transactions.Commands.CreateTransaction;

public sealed record CreateTransactionCommand(string Currency):ICommand<Result<CreatedTransactionResult>>;


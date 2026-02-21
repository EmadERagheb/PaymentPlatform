namespace Transactions.Application.Transactions.Commands.AddTransactionItems;

public sealed record AddTransactionItemsCommand(Guid TransactionId, IEnumerable<AddTransactionItemDto> Items) : ICommand<Result>;
public sealed record AddTransactionItemDto(string Description, int Quantity, decimal UnitPrice);


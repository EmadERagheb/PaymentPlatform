namespace Transactions.Application.Transactions.Commands.AddTransactionItems;

public class AddTransactionItemsCommandHandler(ITransactionDbContext transactionDbContext, ILogger<AddTransactionItemsCommandHandler> logger) : ICommandHandler<AddTransactionItemsCommand, Result>
{
    public async Task<Result> Handle(AddTransactionItemsCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching transaction with ID {TransactionId} to add items.", command.TransactionId);
        var transaction = await transactionDbContext.Transactions.Include(t => t.Items).Where(t => t.Id == TransactionId.Of(command.TransactionId)).FirstOrDefaultAsync(cancellationToken);
        if (transaction is null)
        {
            logger.LogError("Transaction with ID {TransactionId} not found.", command.TransactionId);
            return Result.Failure(new Error("TransactionNotFound", "Transaction not found."));
        }
        foreach (var item in command.Items)
            transaction.AddItem(item.Description, item.Quantity, item.UnitPrice);
        await transactionDbContext.TransactionItems.AddRangeAsync(transaction.Items);
         transactionDbContext.Transactions.Update(transaction);
        logger.LogInformation("Adding {ItemCount} items to transaction {TransactionId}.", command.Items.Count(), command.TransactionId);
        await transactionDbContext.SaveChangesAsync();
        return Result.Success();
    }
}

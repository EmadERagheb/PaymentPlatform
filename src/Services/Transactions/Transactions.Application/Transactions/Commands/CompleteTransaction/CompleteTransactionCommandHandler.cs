namespace Transactions.Application.Transactions.Commands.CompleteTransaction;

public sealed class CompleteTransactionCommandHandler(ITransactionDbContext dbContext, ILogger<CompleteTransactionCommandHandler> logger) : ICommandHandler<CompleteTransactionCommand, Result>
{
    public async Task<Result> Handle(CompleteTransactionCommand command, CancellationToken cancellationToken)
    {
        var transactionId = TransactionId.Of(command.TransactionId);
        logger.LogInformation("Fetching transaction with ID {TransactionId} for payment confirmation", transactionId);
        var transaction = await dbContext.Transactions.FirstOrDefaultAsync(t => t.Id == transactionId);
        if (transaction is null)
        {
            logger.LogWarning("Transaction with ID {TransactionId} not found for payment confirmation", transactionId);
            return Result.Failure(new Error("TransactionNotFound", "Transaction not found."));
        }
        transaction.Complete();
        dbContext.Transactions.Update(transaction);
        await dbContext.SaveChangesAsync();
        return Result.Success();
    }
}

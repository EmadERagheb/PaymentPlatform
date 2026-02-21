namespace Transactions.Application.Transactions.Commands.SubmitTransaction;

public class SubmitTransactionCommandHnadler(ITransactionDbContext transactionDbContext, ILogger<SubmitTransactionCommandHnadler> logger) : ICommandHandler<SubmitTransactionCommand, Result>
{
    public async Task<Result> Handle(SubmitTransactionCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching transaction with ID {TransactionId} to add items.", command.TransactionId);
        var transaction = await transactionDbContext.Transactions.Include(t => t.Items).FirstOrDefaultAsync(t => t.Id == TransactionId.Of(command.TransactionId), cancellationToken);
        if (transaction is null)
        {
            logger.LogError("Transaction with ID {TransactionId} not found.", command.TransactionId);
            return Result.Failure(new Error("TransactionNotFound", "Transaction not found."));
        }
        transaction.Submit();
        logger.LogInformation("Saving submitted transaction with ID {TransactionId}.", command.TransactionId);
        await transactionDbContext.SaveChangesAsync();
        return Result.Success();
    }
}

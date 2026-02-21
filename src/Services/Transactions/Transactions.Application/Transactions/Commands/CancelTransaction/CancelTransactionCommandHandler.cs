namespace Transactions.Application.Transactions.Commands.CancelTransaction;

public sealed class CancelTransactionCommandHandler(ITransactionDbContext transactionDbContext, ILogger<CancelTransactionCommandHandler> logger) : ICommandHandler<CancelTransactionCommand, Result>
{
    public async Task<Result> Handle(CancelTransactionCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching transaction with ID {TransactionId} ", command.TransactionId);
        var transaction = await transactionDbContext.Transactions.Include(t => t.Items).FirstOrDefaultAsync(t => t.Id.Value == command.TransactionId, cancellationToken);
        if (transaction is null)
        {
            logger.LogError("Transaction with ID {TransactionId} not found.", command.TransactionId);
            return Result.Failure(new Error("TransactionNotFound", "Transaction not found."));
        }
        transaction.Cancel();
        logger.LogInformation("Saving canceled transaction with ID {TransactionId}.", command.TransactionId);
        await transactionDbContext.SaveChangesAsync();
        return Result.Success();
    }
}

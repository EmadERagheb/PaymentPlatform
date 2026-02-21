namespace Transactions.Application.Transactions.Commands.CreateTransaction;

public sealed class CreateTransactionCommandHandler(ITransactionDbContext transactionDbContext) : ICommandHandler<CreateTransactionCommand, Result<CreatedTransactionResult>>
{
    public async Task<Result<CreatedTransactionResult>> Handle(CreateTransactionCommand command, CancellationToken cancellationToken)
    {
        var currency = Currency.Of(command.Currency);
        var transaction = new Transaction(currency);
        await transactionDbContext.Transactions.AddAsync(transaction, cancellationToken);
        await transactionDbContext.SaveChangesAsync(cancellationToken);
        return Result.Success(new CreatedTransactionResult(transaction.Id.Value, transaction.TotalAmount.Currency.Code));
    }
}


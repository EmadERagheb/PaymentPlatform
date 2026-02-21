namespace Transactions.Application.Transactions.Commands.SubmitTransaction;

public sealed class SubmitTransactionCommandValidator : AbstractValidator<SubmitTransactionCommand>
{
    public SubmitTransactionCommandValidator()
    {
        RuleFor(x => x.TransactionId)
            .NotEmpty().WithMessage("Transaction ID is required.");
    }
}


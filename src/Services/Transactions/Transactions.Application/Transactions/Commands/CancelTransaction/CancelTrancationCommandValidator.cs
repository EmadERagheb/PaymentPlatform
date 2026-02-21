namespace Transactions.Application.Transactions.Commands.CancelTransaction;

public sealed class CancelTrancationCommandValidator : AbstractValidator<CancelTransactionCommand>
{
    public CancelTrancationCommandValidator()
    {
        RuleFor(x => x.TransactionId)
            .NotEmpty().WithMessage("Transaction ID is required.");
    }
}


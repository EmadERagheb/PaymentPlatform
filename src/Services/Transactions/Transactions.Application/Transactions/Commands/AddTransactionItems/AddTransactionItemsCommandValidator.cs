namespace Transactions.Application.Transactions.Commands.AddTransactionItems;

public class AddTransactionItemsCommandValidator : AbstractValidator<AddTransactionItemsCommand>
{
    public AddTransactionItemsCommandValidator()
    {
        RuleFor(command => command.TransactionId)
            .NotEmpty().WithMessage("Transaction ID is required.");
        RuleFor(command => command.Items)
            .NotEmpty().WithMessage("At least one item is required.")
            .Must(items => items.All(item => item.UnitPrice > 0))
            .WithMessage("All items must have a valid unit price.")
            .Must(items => items.All(item => item.Quantity > 0))
            .WithMessage("All items must have a valid quantity.");
    }
}

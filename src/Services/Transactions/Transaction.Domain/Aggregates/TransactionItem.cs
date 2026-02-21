
namespace Transactions.Domain.Aggregates;

public class TransactionItem : Entity<TransactionItemId>
{
    public TransactionId TransactionId { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal LineTotal => Quantity * UnitPrice;

    private TransactionItem() { }

    internal TransactionItem(TransactionId transactionId, string description, int quantity, decimal unitPrice)
    {
        var errors = new List<ValidationError>();
        if (string.IsNullOrWhiteSpace(description))
            errors.Add(new ValidationError(nameof(description), "Item description is required."));
        if (quantity <= 0)
            errors.Add(new ValidationError(nameof(quantity), "Quantity must be positive."));
        if (unitPrice < 0)
            errors.Add(new ValidationError(nameof(unitPrice), "Unit price cannot be negative."));
        if (transactionId.Value == Guid.Empty)
            errors.Add(new ValidationError(nameof(transactionId), "Transaction ID is required."));
        if (errors.Any())
            throw new ValidationException(errors);

        Id = TransactionItemId.Of(Guid.NewGuid());
        TransactionId = transactionId;
        Description = description;
        Quantity = quantity;
        UnitPrice = unitPrice;


    }
}

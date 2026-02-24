namespace Transactions.Domain.Aggregates;

public class Transaction : AggregateRoot<TransactionId>
{
    private readonly List<TransactionItem> _items = new();
    public TransactionState State { get; private set; }
    public Money TotalAmount { get; private set; }
    public IReadOnlyCollection<TransactionItem> Items => _items.AsReadOnly();
    private Transaction() { }


    public Transaction(Currency currency)
    {
        Id = TransactionId.Of(Guid.NewGuid());
        State = TransactionState.Draft;
        TotalAmount = Money.Zero(currency);
    }
    public void AddItem(string description, int quantity, decimal unitPrice)
    {
        if (State != TransactionState.Draft)
            throw new ValidationException([new ValidationError(nameof(State), "Only draft transactions can add items.")]);
        var item = new TransactionItem(Id, description, quantity, unitPrice);
        _items.Add(item);
        RecalculateTotal(item);
    }

    public void Submit()
    {
        if (State != TransactionState.Draft)
            throw new ValidationException([new ValidationError(nameof(State), "Only draft transactions can be submitted.")]);
        if (!Items.Any())
            throw new ValidationException([new ValidationError(nameof(Items), "Cannot submit a transaction with no items.")]);
        State = TransactionState.Submitted;
        AddDomainEvent(new TransactionSubmitDomainEvent(Id.Value, TotalAmount.Currency.Code, TotalAmount.Amount));
    }
    public void Cancel()
    {

        if (State == TransactionState.Completed)
            throw new ValidationException([new ValidationError(nameof(State), "A completed transaction cannot be cancelled.")]);
        if (State == TransactionState.Cancelled)
            return; // Idempotent
        State = TransactionState.Cancelled;
       
    }
    public void Complete()
    {
        if (State != TransactionState.Submitted)
            throw new ValidationException([new ValidationError(nameof(State), "Only submitted transactions can be completed.")]);
        State = TransactionState.Completed;
       
    
    }


    private void RecalculateTotal(TransactionItem item)
    {
       
        var total = TotalAmount.Amount + item.LineTotal;
        TotalAmount = new Money(total, TotalAmount.Currency);
    }
}

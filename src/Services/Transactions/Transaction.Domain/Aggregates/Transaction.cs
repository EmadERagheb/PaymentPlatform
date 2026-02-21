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
       var item = new TransactionItem(Id, description, quantity, unitPrice);
        _items.Add(item);
        RecalculateTotal();
    }
   
    public void Submit()
    {
        if (State != TransactionState.Draft)
            throw new ValidationException([new ValidationError(nameof(State), "Only draft transactions can be submitted.")]);
        if (!Items.Any())
            throw new ValidationException([new ValidationError(nameof(Items), "Cannot submit a transaction with no items.")]);
        State = TransactionState.Submitted;
        AddDomainEvent(new TransactionSubmittedEvent(this));
    }
    public void Cancel()
    {
        if (State != TransactionState.Submitted)
            throw new ValidationException([new ValidationError(nameof(State), "Only submitted transactions can be cancelled.")]);
        State = TransactionState.Cancelled;
        AddDomainEvent(new TransactionCancelledEvent(this));
    }
    public void Complete()
    {
        if (State != TransactionState.Submitted)
            throw new ValidationException([new ValidationError(nameof(State), "Only submitted transactions can be completed.")]);
        State = TransactionState.Completed;
        AddDomainEvent(new TransactionCompletedEvent(this));
    }

    public void UpdateItem(TransactionItem item)
    {
        var existingItem = _items.FirstOrDefault(i => i.Id.Value == item.Id.Value);
        if (existingItem == null)
            throw new ValidationException([new ValidationError(nameof(item.Id), "Transaction item not found.")]);
        _items.Remove(existingItem);
        _items.Add(item);
        RecalculateTotal();
    }
    private void RecalculateTotal()
    {
        TotalAmount = new Money(_items.Sum(i => i.LineTotal), TotalAmount.Currency);
    }
}

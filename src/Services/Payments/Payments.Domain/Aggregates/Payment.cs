namespace Payments.Domain.Aggregates;

public sealed class Payment : AggregateRoot<PaymentId>
{
    public TransactionId TransactionId { get; set; }
    public Money TotalAmount { get; private set; } = default!;
    public PaymentState State { get; private set; }
    public string? FailureReason { get; private set; }
    private Payment() { }
    public static Payment Start(TransactionId transactionId, Money totalAmount)
    {
        if (transactionId == null || transactionId.Value == Guid.Empty)
            throw new ValidationException([new ValidationError(nameof(transactionId), "Transaction ID cannot be empty.")]);
        if (totalAmount == null || totalAmount.Amount <= 0)
            throw new ValidationException([new ValidationError(nameof(totalAmount), "Total amount must be greater than zero.")]);
        return new Payment
        {
            Id = PaymentId.Of(Guid.NewGuid()),
            TransactionId = transactionId,
            TotalAmount = totalAmount,
            State = PaymentState.Pending
        };
    }
    public void Confirm()
    {
        if (State != PaymentState.Pending)
            throw new ValidationException([new ValidationError(nameof(State), "Only a pending payment can be confirmed.")]);
        State = PaymentState.Confirmed;
       AddDomainEvent(new PaymentConfirmedDomainEvent(Id.Value, TransactionId.Value,TotalAmount.Currency.Code, TotalAmount.Amount));
    }

    public void Fail(string reason)
    {
        if (State != PaymentState.Pending)
            throw new ValidationException([new ValidationError(nameof(State), "Only a pending payment can be failed.")]
            );

        State = PaymentState.Failed;
        FailureReason = reason ?? string.Empty;
        AddDomainEvent(new PaymentFailedDomainEvent(Id.Value, TransactionId.Value, TotalAmount.Currency.Code, TotalAmount.Amount, FailureReason));
    }

}

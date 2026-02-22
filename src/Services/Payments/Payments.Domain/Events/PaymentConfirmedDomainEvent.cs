namespace Payments.Domain.Events;

public sealed record PaymentConfirmedDomainEvent(Guid PaymentId,Guid TransactionId,string Currency,decimal Amount) : IDomainEvent;

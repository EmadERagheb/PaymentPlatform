namespace Payments.Domain.Events;

public  record PaymentFailedDomainEvent(Guid PaymentId, Guid TransactionId, string Currency, decimal Amount,string FailureReason) : IDomainEvent;


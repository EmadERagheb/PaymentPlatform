namespace Payments.Domain.Events;

public sealed record PaymentFailedEvent(Payment Payment) : IDomainEvent;


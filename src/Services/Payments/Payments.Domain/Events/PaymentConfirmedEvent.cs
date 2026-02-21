namespace Payments.Domain.Events;

public sealed record PaymentConfirmedEvent(Payment Payment) : IDomainEvent;

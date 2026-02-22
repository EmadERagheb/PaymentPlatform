

namespace BuildingBlocks.Messaging.Events;

public record FailedPaymentEvent(Guid PaymentId, Guid TransactionId, string Currency, decimal Amount, string FailureReason) : IntegrationEvent;




namespace BuildingBlocks.Messaging.Events;

public record PaymentConfirmedEvent(Guid PaymentId, Guid TransactionId, string Currency, decimal Amount, string? CorrelationId = null) : IntegrationEvent(CorrelationId);


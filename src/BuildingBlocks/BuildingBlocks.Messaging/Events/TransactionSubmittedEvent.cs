namespace BuildingBlocks.Messaging.Events;

public record TransactionSubmittedEvent(Guid TransactionId, string Currency, decimal Amount, string? CorrelationId = null) : IntegrationEvent(CorrelationId);


namespace BuildingBlocks.Messaging.Events;
public record IntegrationEvent(string? CorrelationId = null)
{

    public string? CorrelationId { get; init; } = CorrelationId;
    public Guid Id => Guid.NewGuid();
    public DateTime OccurredOn => DateTime.UtcNow;
    public string EventType => GetType().AssemblyQualifiedName;
}

namespace BuildingBlocks.Messaging.Correlation;

public interface ICorrelationIdAccessor
{
    string? GetCorrelationId();
}

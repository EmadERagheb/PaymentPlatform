namespace BuildingBlocks.Messaging.Correlation;

/// <summary>Provides the current request's correlation ID for distributed tracing (e.g. when publishing to MQ).</summary>
public interface ICorrelationIdAccessor
{
    string? GetCorrelationId();
}

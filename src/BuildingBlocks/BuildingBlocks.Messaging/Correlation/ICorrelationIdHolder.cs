namespace BuildingBlocks.Messaging.Correlation;

/// <summary>Holds the current correlation ID for the scope (e.g. set by outbox job when processing a message).</summary>
public interface ICorrelationIdHolder
{
    string? Current { get; set; }
}

namespace BuildingBlocks.Messaging.Correlation;

public interface ICorrelationIdHolder
{
    string? Current { get; set; }
}

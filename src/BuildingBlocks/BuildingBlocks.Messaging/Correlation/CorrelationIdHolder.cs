namespace BuildingBlocks.Messaging.Correlation;

public class CorrelationIdHolder : ICorrelationIdHolder
{
    public string? Current { get; set; }
}

namespace Transactions.Infrastructure.Outbox;


public class OutboxOptions
{
    public const string SectionName = "Outbox";
    public int IntervalInSeconds { get; init; }

    public int BatchSize { get; init; }
}

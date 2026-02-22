using System.ComponentModel.DataAnnotations;
namespace Transactions.Infrastructure.Outbox;


public sealed class OutboxMessage
{
    public OutboxMessage(Guid id, DateTime occurredOnUtc, string type, string content)
    {
        Id = id;
        OccurredOnUtc = occurredOnUtc;
        Content = content;
        Type = type;
    }

    public Guid Id { get; set; }

    public DateTime OccurredOnUtc { get; set; }

    public string Type { get; set; }

    public string Content { get; set; }

    public DateTime? ProcessedOnUtc { get; set; }

    public string? Error { get; set; }
    [Timestamp]
    public byte[] Version { get; set; } = [];
}

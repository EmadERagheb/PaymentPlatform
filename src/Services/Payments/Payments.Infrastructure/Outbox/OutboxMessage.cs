using System.ComponentModel.DataAnnotations;

namespace Payments.Infrastructure.Outbox;

public sealed class OutboxMessage
{
    public OutboxMessage(Guid id, DateTime occurredOnUtc, string type, string content, string? correlationId = null)
    {
        Id = id;
        OccurredOnUtc = occurredOnUtc;
        Content = content;
        Type = type;
        CorrelationId = correlationId;

    }

    public string? CorrelationId { get; set; }

    public Guid Id { get; set; }

    public DateTime OccurredOnUtc { get; set; }

    public string Type { get; set; }

    public string Content { get; set; }

    public DateTime? ProcessedOnUtc { get; set; }

    public string? Error { get; set; }
    [Timestamp]
    public byte[] Version { get; set; } = [];
}

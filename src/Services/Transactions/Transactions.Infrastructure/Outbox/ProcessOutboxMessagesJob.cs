using BuildingBlocks.Abstractions;
using BuildingBlocks.Messaging.Correlation;
using Microsoft.Extensions.Options;
using Serilog.Context;

namespace Transactions.Infrastructure.Outbox;

[DisallowConcurrentExecution]
public class ProcessOutboxMessagesJob(TransactionsDbContext dbContext, IOptions<OutboxOptions> options,ILogger<ProcessOutboxMessagesJob> logger,IPublisher publisher,
    ICorrelationIdHolder correlationIdHolder) : IJob
{
    private readonly OutboxOptions _options = options.Value;
    private static readonly JsonSerializerSettings JsonSerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All
    };
    public async Task Execute(IJobExecutionContext context)
    {
        var outboxMessages = await dbContext.OutboxMessages.Where(m => !m.ProcessedOnUtc.HasValue).Take(_options.BatchSize).ToListAsync();
        if (outboxMessages.Count == 0)
            return;

        logger.LogInformation("Starting to process {Count} outbox messages at {Time}", outboxMessages.Count, DateTime.UtcNow);
        foreach (var message in outboxMessages)
        {
            using (LogContext.PushProperty("CorrelationId", message.CorrelationId ?? "outbox"))
            {
                try
                {
                    var domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(message.Content, JsonSerializerSettings);
                    if (domainEvent != null)
                    {
                        correlationIdHolder.Current = message.CorrelationId;
                        await publisher.Publish(domainEvent);
                        message.ProcessedOnUtc = DateTime.UtcNow;
                        logger.LogInformation("Successfully processed outbox message {MessageId}", message.Id);
                    }
                    else
                    {
                        logger.LogWarning("Failed to deserialize outbox message {MessageId}. Skipping.", message.Id);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing outbox message {MessageId}", message.Id);
                    message.Error = ex.Message;
                }
            }
        }
        dbContext.OutboxMessages.UpdateRange(outboxMessages);
        dbContext.SaveChanges();
        logger.LogInformation("Finished processing outbox messages at {Time}", DateTime.UtcNow);
    }
}

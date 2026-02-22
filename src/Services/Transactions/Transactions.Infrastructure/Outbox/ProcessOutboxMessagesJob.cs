using BuildingBlocks.Abstractions;
using Microsoft.Extensions.Options;
namespace Transactions.Infrastructure.Outbox;

[DisallowConcurrentExecution]
public class ProcessOutboxMessagesJob(TransactionsDbContext dbContext, IOptions<OutboxOptions> options, ILogger<ProcessOutboxMessagesJob> logger, IPublisher publisher) : IJob
{
    private readonly OutboxOptions _options = options.Value;
    private static readonly JsonSerializerSettings JsonSerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All
    };
    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("Starting to process outbox messages at {Time}", DateTime.UtcNow);
        var outboxMessages = await dbContext.OutboxMessages.Where(m => !m.ProcessedOnUtc.HasValue).Take(_options.BatchSize).ToListAsync();
        logger.LogInformation("Found {Count} unprocessed outbox messages", outboxMessages.Count);
        if (outboxMessages.Count == 0)
        {
            logger.LogInformation("No unprocessed outbox messages found. Ending job execution.");
            return;
        }
        foreach (var message in outboxMessages)
        {
            try
            {


                var domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(message.Content, JsonSerializerSettings);
                if (domainEvent != null)
                {
                    await publisher.Publish(domainEvent);
                    message.ProcessedOnUtc = DateTime.UtcNow;
                    logger.LogInformation("Successfully processed outbox message with ID {MessageId}", message.Id);
                }
                else
                {
                    logger.LogWarning("Failed to deserialize outbox message with ID {MessageId}. Skipping.", message.Id);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing outbox message with ID {MessageId}", message.Id);
                message.Error = ex.Message;
            }
        }
        dbContext.OutboxMessages.UpdateRange(outboxMessages);
        dbContext.SaveChanges();
        logger.LogInformation("Finished processing outbox messages at {Time}", DateTime.UtcNow);

    }
}

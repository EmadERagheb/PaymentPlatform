using BuildingBlocks.Messaging.Correlation;
using Newtonsoft.Json;
namespace Payments.Infrastructure.Interceptors;

public class AddOutboxMessagesInterceptor(ICorrelationIdAccessor correlationIdAccessor) : SaveChangesInterceptor
{
    private static readonly JsonSerializerSettings JsonSerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All
    };
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        AddDomainEventsAsOutboxMessages(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void AddDomainEventsAsOutboxMessages(DbContext? context)
    {
        if (context == null)
            return;
        var aggregates = context.ChangeTracker
            .Entries<IAggregateRoot>()
            .Where(a => a.Entity.DomainEvents.Any())
            .Select(a => a.Entity);
        var correlationId = correlationIdAccessor.GetCorrelationId();
        var outboxMessages = aggregates
            .SelectMany(a => a.DomainEvents)
            .Select(domainEvent => new OutboxMessage(
                Guid.NewGuid(),
                DateTime.UtcNow,
                domainEvent.GetType().Name,
                SerializeEventContent(domainEvent), correlationId))
            .ToList();
        aggregates.ToList().ForEach(a => a.ClearDomainEvents());

        context.Set<OutboxMessage>().AddRange(outboxMessages);
    }

    private static string SerializeEventContent(IDomainEvent domainEvent)
    {
        return JsonConvert.SerializeObject(domainEvent, JsonSerializerSettings);
    }
}

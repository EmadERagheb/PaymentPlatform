
using BuildingBlocks.Messaging.Correlation;
using BuildingBlocks.Messaging.Events;
using MassTransit;
using Transactions.Domain.Events;

namespace Transactions.Application.Transactions.Events.Domain;

public class TransactionSubmittedEventHandler(
    IPublishEndpoint publishEndpoint,
    ICorrelationIdAccessor correlationIdAccessor,
    ILogger<TransactionSubmittedEventHandler> logger) : INotificationHandler<TransactionSubmitDomainEvent>
{
    public Task Handle(TransactionSubmitDomainEvent transactionSubmitDomainEvent, CancellationToken cancellationToken)
    {
        logger.LogInformation("Publishing transaction submitted event: {@Event}", transactionSubmitDomainEvent);
        var correlationId = correlationIdAccessor.GetCorrelationId();
        var eventMessage = new TransactionSubmittedEvent(
            transactionSubmitDomainEvent.TransactionId,
            transactionSubmitDomainEvent.Currency,
            transactionSubmitDomainEvent.TotalAmount,
            correlationId);
        return publishEndpoint.Publish(eventMessage, cancellationToken);
    }
}

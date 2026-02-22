
using BuildingBlocks.Messaging.Events;
using MassTransit;
using Transactions.Domain.Events;

namespace Transactions.Application.Transactions.Events.Domain;

public class TransactionSubmittedEventHandler(IPublishEndpoint publishEndpoint, ILogger<TransactionSubmittedEventHandler> logger) : INotificationHandler<TransactionSubmitDomainEvent>
{
    public Task Handle(TransactionSubmitDomainEvent transactionSubmitDomainEvent, CancellationToken cancellationToken)
    {
        logger.LogInformation("Publishing transaction submitted event: {@Event}", transactionSubmitDomainEvent);
        var eventMessage = new TransactionSubmittedEvent(transactionSubmitDomainEvent.TransactionId, transactionSubmitDomainEvent.Currency, transactionSubmitDomainEvent.TotalAmount);
        return publishEndpoint.Publish(eventMessage, cancellationToken);
    }
}

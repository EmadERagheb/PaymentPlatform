

using BuildingBlocks.Messaging.Correlation;
using Payments.Domain.Events;

namespace Payments.Application.Payments.Event.Domain;

public class ConfirmPaymentDomainEventHandler(IPublishEndpoint publishEndpoint, ILogger<ConfirmPaymentDomainEventHandler> logger,
     ICorrelationIdAccessor correlationIdAccessor) : INotificationHandler<PaymentConfirmedDomainEvent>
{
    public async Task Handle(PaymentConfirmedDomainEvent paymentConfirmedDomainEvent, CancellationToken cancellationToken)
    {
        logger.LogInformation("Publishing transaction submitted event: {@Event}", paymentConfirmedDomainEvent);
        var correlationId = correlationIdAccessor.GetCorrelationId();
        logger.LogInformation("Publishing payment confirmed event: {@Event}", paymentConfirmedDomainEvent);
        var eventMessage = new PaymentConfirmedEvent(paymentConfirmedDomainEvent.PaymentId, paymentConfirmedDomainEvent.TransactionId, paymentConfirmedDomainEvent.Currency, paymentConfirmedDomainEvent.Amount,correlationId);
        await publishEndpoint.Publish(eventMessage, cancellationToken);
    }
}

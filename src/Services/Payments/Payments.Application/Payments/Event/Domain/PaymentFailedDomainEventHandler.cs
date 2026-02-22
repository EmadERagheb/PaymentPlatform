using BuildingBlocks.Messaging.Correlation;

namespace Payments.Application.Payments.Event.Domain;

public class PaymentFailedDomainEventHandler(IPublishEndpoint publishEndpoint, ILogger<PaymentFailedDomainEventHandler> logger,
     ICorrelationIdAccessor correlationIdAccessor) : INotificationHandler<PaymentFailedDomainEvent>
{
    public async Task Handle(PaymentFailedDomainEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Publishing payment failed event: {@Event}", notification);
        var correlationId = correlationIdAccessor.GetCorrelationId();
        var eventMessage = new FailedPaymentEvent(notification.PaymentId, notification.TransactionId, notification.Currency, notification.Amount, notification.FailureReason, correlationId);
        await publishEndpoint.Publish(eventMessage, cancellationToken);
    }
}

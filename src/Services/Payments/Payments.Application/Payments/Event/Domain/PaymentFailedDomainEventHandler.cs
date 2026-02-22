namespace Payments.Application.Payments.Event.Domain;

public class PaymentFailedDomainEventHandler(IPublishEndpoint publishEndpoint, ILogger<PaymentFailedDomainEventHandler> logger) : INotificationHandler<PaymentFailedDomainEvent>
{
    public async Task Handle(PaymentFailedDomainEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Publishing payment failed event: {@Event}", notification);
        var eventMessage = new FailedPaymentEvent(notification.PaymentId, notification.TransactionId, notification.Currency, notification.Amount, notification.FailureReason);
        await publishEndpoint.Publish(eventMessage, cancellationToken);
    }
}

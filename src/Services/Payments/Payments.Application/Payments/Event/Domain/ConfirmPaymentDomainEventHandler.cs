

using Payments.Domain.Events;

namespace Payments.Application.Payments.Event.Domain;

public class ConfirmPaymentDomainEventHandler(IPublishEndpoint publishEndpoint, ILogger<ConfirmPaymentDomainEventHandler> logger) : INotificationHandler<PaymentConfirmedDomainEvent>
{
    public async Task Handle(PaymentConfirmedDomainEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Publishing payment confirmed event: {@Event}", notification);
        var eventMessage = new PaymentConfirmedEvent(notification.PaymentId, notification.TransactionId, notification.Currency, notification.Amount);
        await publishEndpoint.Publish(eventMessage, cancellationToken);
    }
}

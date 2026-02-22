using Transactions.Application.Transactions.Commands.CompleteTransaction;
namespace Transactions.Application.Transactions.Events.Integrations;

public class PaymentFailedEventHandler(ILogger<PaymentFailedEventHandler> logger, ISender mediator) : IConsumer<FailedPaymentEvent>
{
    public async Task Consume(ConsumeContext<FailedPaymentEvent> context)
    {
        var correlationId = context.Message.CorrelationId ?? "transactions-consumer";
        using (Serilog.Context.LogContext.PushProperty("CorrelationId", correlationId))
        {
            logger.LogInformation("Payment failed event received: {@Event}", context.Message);
            var command = new CompleteTransactionCommand(context.Message.TransactionId);
            await mediator.Send(command);
        }
    }
}

using Transactions.Application.Transactions.Commands.CompleteTransaction;

namespace Transactions.Application.Transactions.Events.Integrations;

public class PaymentConfirmedEventHandler(ILogger<PaymentConfirmedEventHandler> logger, ISender mediator) : IConsumer<PaymentConfirmedEvent>
{
    public async Task Consume(ConsumeContext<PaymentConfirmedEvent> context)
    {
        var correlationId = context.Message.CorrelationId ?? "transactions-consumer";
        using (Serilog.Context.LogContext.PushProperty("CorrelationId", correlationId))
        {
            logger.LogInformation("Payment confirmed event received: {@Event}", context.Message);
            var command = new CompleteTransactionCommand(context.Message.TransactionId);
            await mediator.Send(command);
        }
    }
}

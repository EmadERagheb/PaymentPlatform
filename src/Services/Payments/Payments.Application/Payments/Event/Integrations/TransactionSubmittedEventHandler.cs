
using Payments.Application.Payments.StartPayment;
using Serilog.Context;

namespace Payments.Application.Payments.Event.Integrations;

public class TransactionSubmittedEventHandler(ISender sender, ILogger<TransactionSubmittedEventHandler> logger) : IConsumer<TransactionSubmittedEvent>
{
    public async Task Consume(ConsumeContext<TransactionSubmittedEvent> context)
    {
        var correlationId = context.Message.CorrelationId ?? "payments-consumer";
        using (Serilog.Context.LogContext.PushProperty("CorrelationId", correlationId))
        {
            logger.LogInformation("Received TransactionSubmittedEvent: {@Event}", context.Message);
            var command = new StartPaymentCommand(context.Message.TransactionId, context.Message.Amount, context.Message.Currency);
            await sender.Send(command);
        }
    }
}

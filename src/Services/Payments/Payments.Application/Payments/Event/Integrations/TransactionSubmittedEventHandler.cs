
using Payments.Application.Payments.StartPayment;
namespace Payments.Application.Payments.Event.Integrations;

public class TransactionSubmittedEventHandler(ISender sender, ILogger<TransactionSubmittedEventHandler> logger) : IConsumer<TransactionSubmittedEvent>
{
    public async Task Consume(ConsumeContext<TransactionSubmittedEvent> context)
    {
     logger.LogInformation("Received TransactionSubmittedEvent: {@Event}", context.Message);
        var command = new StartPaymentCommand(context.Message.TransactionId, context.Message.Amount, context.Message.Currency);
         await sender.Send(command);
    }
}

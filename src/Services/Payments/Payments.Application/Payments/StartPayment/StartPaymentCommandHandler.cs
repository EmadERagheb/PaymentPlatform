namespace Payments.Application.Payments.StartPayment;

public sealed class StartPaymentCommandHandler(IPaymentDbContext dbContext, ILogger<StartPaymentCommandHandler> logger) : ICommandHandler<StartPaymentCommand, Result<StartedPaymentResponse>>
{
    public async Task<Result<StartedPaymentResponse>> Handle(StartPaymentCommand command, CancellationToken cancellationToken)
    {
        var currency = Currency.Of(command.Currency);
        var money = new Money(command.Amount, currency);
        var payment = Payment.Start(TransactionId.Of(command.TransactionId), money);
        dbContext.Payments.Add(payment);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success(new StartedPaymentResponse(payment.Id.Value, payment.TransactionId.Value, payment.TotalAmount.Amount, payment.TotalAmount.Currency.Code));
    }
}

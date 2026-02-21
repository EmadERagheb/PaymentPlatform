namespace Payments.Application.Payments.ConfirmPayment;

public sealed class ConfirmPaymentCommandHandler(IPaymentDbContext paymentDbContext, ILogger<ConfirmPaymentCommandHandler> logger) : ICommandHandler<ConfirmPaymentCommand, Result>
{
    public async Task<Result> Handle(ConfirmPaymentCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching payment with ID {PaymentId} for confirmation.", command.PaymentId);
        var payment = await paymentDbContext.Payments
               .FirstOrDefaultAsync(p => p.Id == PaymentId.Of(command.PaymentId), cancellationToken);
        if (payment is null)
        {
            logger.LogWarning("Payment with ID {PaymentId} not found.", command.PaymentId);
            return Result.Failure(new Error("PaymentNotFound", "Payment not found."));
        }
        payment.Confirm();
        paymentDbContext.Payments.Update(payment);
        logger.LogInformation("Payment with ID {PaymentId} confirmed successfully.", command.PaymentId);
        await paymentDbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

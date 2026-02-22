namespace Payments.Application.Payments.FailPayment;

public sealed class FailPaymentCommandHandler(IPaymentDbContext paymentDbContext, ILogger<FailPaymentCommandHandler> logger) : ICommandHandler<FailPaymentCommand, Result>
{
    public async Task<Result> Handle(FailPaymentCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching payment with ID {PaymentId} for confirmation.", command.PaymentId);
        var payment = await paymentDbContext.Payments
               .FirstOrDefaultAsync(p => p.Id == PaymentId.Of(command.PaymentId), cancellationToken);
        if (payment is null)
        {
            logger.LogWarning("Payment with ID {PaymentId} not found.", command.PaymentId);
            return Result.Failure(new Error("PaymentNotFound", "Payment not found."));
        }
        payment.Fail(command.FailureReson ?? "Payment failed due to an unknown reason.");
        paymentDbContext.Payments.Update(payment);
        logger.LogInformation("Payment with ID {PaymentId} failed successfully.", command.PaymentId);
        await paymentDbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

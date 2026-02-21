namespace Payments.Application.Payments.StartPayment;

public sealed record StartedPaymentResponse(Guid PaymentId, Guid TransactionId, decimal TotalAmount, string Currency);



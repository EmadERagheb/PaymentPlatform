namespace Payments.API.Controllers;

[Route("api/payments")]
[ApiController]
public class PaymentsController(IMediator mediator) : ControllerBase
{
    /// <summary>Start a payment for a transaction (idempotent by transaction id).</summary>
    [HttpPost("start")]
    public async Task<IActionResult> Start([FromBody] StartPaymentCommand request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        if (result.IsFailure)
            return BadRequest(result.Error);
        return CreatedAtAction(nameof(Start), result.Value);
    }
    /// <summary>Confirm a payment. Publishes PaymentConfirmed.</summary>
    [HttpPost("{id:guid}/confirm")]
    public async Task<IActionResult> Confirm(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new ConfirmPaymentCommand(id), cancellationToken);
        if (result.IsFailure)
            return BadRequest(result.Error);
        return NoContent();
    }
    /// <summary>Fail a payment. Publishes PaymentFailed.</summary>
    [HttpPost("{id:guid}/fail")]
    public async Task<IActionResult> Fail(Guid id, [FromBody] FailPaymentCommand request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        if (result.IsFailure)
            return BadRequest(result.Error);
        return NoContent();
    }
}

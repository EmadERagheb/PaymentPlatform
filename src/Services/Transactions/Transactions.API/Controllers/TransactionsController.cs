namespace Transactions.API.Controllers;

[Route("api/transactions")]
[ApiController]
public class TransactionsController(IMediator mediator) : ControllerBase
{
    /// <summary>Create a new transaction (Draft).</summary>
    [HttpPost]
    public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        if (result.IsSuccess)
            return CreatedAtAction(nameof(CreateTransaction), result.Value);
        return BadRequest(result.Error);
    }
    /// <summary>Add items to a draft transaction.</summary>
    [HttpPost("{id:guid}/items")]
    public async Task<IActionResult> AddTransactionItems(Guid id, [FromBody] AddTransactionItemsCommand request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        if (result.IsSuccess)
            return NoContent();
        return BadRequest(result.Error);
    }
    /// <summary>Submit transaction. Publishes TransactionSubmitted event.</summary>
    [HttpPost("{id:guid}/submit")]
    public async Task<IActionResult> SubmitTransaction(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new SubmitTransactionCommand(id), cancellationToken);
        if (result.IsSuccess)
            return NoContent();
        return BadRequest(result.Error);
    }
    /// <summary>Cancel a draft or submitted transaction.</summary>
    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> CancelTransaction(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CancelTransactionCommand(id), cancellationToken);
        if (result.IsSuccess)
            return NoContent();
        return BadRequest(result.Error);
    }
}

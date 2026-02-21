namespace Transactions.API.Controllers
{
    [Route("errors/{code}")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorsController(ILogger<ErrorsController> logger) : ControllerBase
    {
        public ActionResult Error(int code)
        {
            logger.LogWarning("Returning error response with code {Code}", code);
            var errorResponse = code switch
            {
                400 => new Error("Error.BadRequest", "The request is invalid."),
                401 => new Error("Error.Unauthorized", "You are not authorized to access this resource."),
                403 => new Error("Error.Forbidden", "You do not have permission to perform this action."),
                404 => new Error("Error.NotFound", "The requested resource was not found."),
                405 => new Error("Error.MethodNotAllowed", "The HTTP method is not allowed."),
                500 => new Error("Error.InternalServerError", "An unexpected error occurred."),
                _ => new Error("Error.InternalServerError", "Unhandled error.")
            };

            return new ObjectResult(errorResponse);
        }
    }
}

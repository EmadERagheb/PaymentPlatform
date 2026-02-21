using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace BuildingBlocks.Exceptions
{
    public class CustomExceptionHandler(RequestDelegate next, ILogger<CustomExceptionHandler> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (OperationCanceledException operationCanceledException)
            {
                // Handle request cancellation (client disconnect, timeout, etc.)
                logger.LogInformation("Request was cancelled: {Message}", operationCanceledException.Message);

                // Only set status code if response hasn't started
                if (!context.Response.HasStarted)
                {
                    context.Response.StatusCode = StatusCodes.Status499ClientClosedRequest;
                }
            }
            catch (Exception exception)
            {
                if (exception is not ValidationException)
                    logger.LogError(exception, "Exception occurred: {Message}", exception.Message);


                var exceptionDetails = GetExceptionDetails(exception);
                var problemDetails = new ProblemDetails
                {
                    Status = exceptionDetails.Status,
                    Type = exceptionDetails.Type,
                    Title = exceptionDetails.Title,
                    Detail = exceptionDetails.Detail,
                    Instance = context.Request.Path
                };
                if (exceptionDetails.Errors is not null)
                {
                    problemDetails.Extensions.Add("errors", exceptionDetails.Errors);
                }
                context.Response.StatusCode = exceptionDetails.Status;
                await context.Response.WriteAsJsonAsync(problemDetails);
            }
        }

        private static ExceptionDetails GetExceptionDetails(Exception exception)
        {
            return exception switch
            {
                ValidationException validationException => new ExceptionDetails(StatusCodes.Status400BadRequest, "ValidationFailure", "Validation Error", "One or more validation errors has occurred", validationException.Errors),
                _ => new ExceptionDetails(StatusCodes.Status500InternalServerError, "ServerError", exception.Message, "An unexpected error has occurred", null)
            };
        }
    }
    internal record ExceptionDetails(int Status, string Type, string Title, string Detail, IEnumerable<object>? Errors);

}


namespace BuildingBlocks.Behaviors;

public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
   where TRequest : IBaseRequest
   where TResponse : Result
{

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        try
        {
            logger.LogInformation("Executing request {RequestName}", requestName);
            var result = await next(cancellationToken);
            if (result.IsSuccess)
            {
                logger.LogInformation("Request {RequestName} processed successfully", requestName);
            }
            else
            {
                using (LogContext.PushProperty("Error", result.Error, true))
                {
                    logger.LogWarning("Request {RequestName} processed with error", requestName);
                }
            }
            return result;
        }

        catch (ValidationException validationException)
        {
            var errors = validationException.Errors.ToList();
            // Serialize errors to JSON for better searchability in Grafana/Loki
            var errorsJson = JsonSerializer.Serialize(errors);
            using (LogContext.PushProperty("ValidationErrors", errorsJson, true))
            {
                logger.LogWarning("Request {RequestName} failed validation with {ErrorCount} error(s)", requestName, errors.Count);
            }
            throw;
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Request {RequestName} was cancelled", requestName);
            throw;
        }

        catch (Exception exception)
        {
            logger.LogError(exception, "Request {RequestName} failed with exception", requestName);
            throw;
        }
    }
}

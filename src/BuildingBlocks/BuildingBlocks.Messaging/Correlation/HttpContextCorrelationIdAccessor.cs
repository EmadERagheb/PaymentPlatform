using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Messaging.Correlation;

public class HttpContextCorrelationIdAccessor(IHttpContextAccessor httpContextAccessor, ICorrelationIdHolder holder) : ICorrelationIdAccessor
{
    public string? GetCorrelationId() =>
        httpContextAccessor.HttpContext?.Items["CorrelationId"]?.ToString()
        ?? holder.Current;
}

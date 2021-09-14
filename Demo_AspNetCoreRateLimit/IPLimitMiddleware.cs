
using AspNetCoreRateLimit;
using Microsoft.Extensions.Options;

namespace Demo_AspNetCoreRateLimit;
public class IPLimitMiddleware : IpRateLimitMiddleware
{
    public IPLimitMiddleware(RequestDelegate next, IProcessingStrategy processingStrategy,
        IOptions<IpRateLimitOptions> options,
        IRateLimitCounterStore counterStore,
        IIpPolicyStore policyStore,
        IRateLimitConfiguration config,
        ILogger<IpRateLimitMiddleware> logger) :
        base(next, processingStrategy, options, counterStore, policyStore, config, logger)
    {
    }

    public override Task ReturnQuotaExceededResponse(HttpContext httpContext, RateLimitRule rule, string retryAfter)
    {
        httpContext.Response.Headers.Append("Access-Control-Allow-Origin", "*");
        return base.ReturnQuotaExceededResponse(httpContext, rule, retryAfter);
    }
}
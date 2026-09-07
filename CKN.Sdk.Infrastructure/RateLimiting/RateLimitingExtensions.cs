using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;

namespace CKN.Sdk.Infrastructure.RateLimiting;

/// <summary>
/// Extension methods for configuring Rate Limiting using .NET built-in middleware.
/// </summary>
public static class RateLimitingExtensions
{
    private const string GlobalFixedWindowPolicy = "GlobalFixedWindow";
    private const string GlobalTokenBucketPolicy = "GlobalTokenBucket";

    /// <summary>
    /// Adds standard rate limiting policies to the service collection.
    /// </summary>
    public static IServiceCollection AddCknRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.OnRejected = async (context, token) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.HttpContext.Response.ContentType = "application/json";
                await context.HttpContext.Response.WriteAsync(
                    "{\"error\": \"Too many requests. Please try again later.\"}", 
                    token);
            };

            // Fixed Window policy (e.g. 100 requests per minute)
            options.AddFixedWindowLimiter(policyName: GlobalFixedWindowPolicy, fixedWindowOptions =>
            {
                fixedWindowOptions.PermitLimit = 100;
                fixedWindowOptions.Window = System.TimeSpan.FromMinutes(1);
                fixedWindowOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                fixedWindowOptions.QueueLimit = 2;
            });

            // Token Bucket policy (allows bursts)
            options.AddTokenBucketLimiter(policyName: GlobalTokenBucketPolicy, tokenBucketOptions =>
            {
                tokenBucketOptions.TokenLimit = 100;
                tokenBucketOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                tokenBucketOptions.QueueLimit = 2;
                tokenBucketOptions.ReplenishmentPeriod = System.TimeSpan.FromSeconds(10);
                tokenBucketOptions.TokensPerPeriod = 10;
                tokenBucketOptions.AutoReplenishment = true;
            });
        });

        return services;
    }
}

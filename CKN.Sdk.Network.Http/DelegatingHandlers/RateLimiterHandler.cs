using System;
using System.Net.Http;
using System.Threading;
using System.Threading.RateLimiting;
using System.Threading.Tasks;
using CKN.Sdk.Core.Common.Results;
using CKN.Sdk.Network.Options;

namespace CKN.Sdk.Network.Http.DelegatingHandlers;

/// <summary>
/// Applies client-side token-bucket rate limiting to all outgoing requests.
/// Requests that cannot be granted a token within the queue limit fail immediately
/// with HTTP 429 so that callers receive a <see cref="Result{T}"/> failure
/// rather than a long queue wait.
/// </summary>
internal sealed class RateLimiterHandler : DelegatingHandler, IDisposable
{
    private readonly TokenBucketRateLimiter _rateLimiter;

    internal RateLimiterHandler(CknRateLimiterOptions options)
    {
        _rateLimiter = new TokenBucketRateLimiter(new TokenBucketRateLimiterOptions
        {
            TokenLimit = options.RequestsPerPeriod,
            TokensPerPeriod = options.RequestsPerPeriod,
            ReplenishmentPeriod = options.Period,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = options.QueueLimit,
            AutoReplenishment = true
        });
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        using var lease = await _rateLimiter.AcquireAsync(
            permitCount: 1,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        if (!lease.IsAcquired)
        {
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.TooManyRequests)
            {
                ReasonPhrase = "Client-side rate limit exceeded"
            };
            return response;
        }

        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _rateLimiter.Dispose();
        }
        base.Dispose(disposing);
    }
}

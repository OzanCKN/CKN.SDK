using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CKN.Sdk.Network.Http.DelegatingHandlers;

/// <summary>
/// Intercepts outgoing requests and replaces sensitive query-parameter values
/// with <c>***REDACTED***</c> in structured log output.
/// The original request URL is never modified; only the logged representation is masked.
/// </summary>
internal sealed class SensitiveQueryMaskingHandler : DelegatingHandler
{
    private readonly IReadOnlyList<string> _sensitiveParams;
    private readonly ILogger _logger;
    private readonly string _clientName;

    internal SensitiveQueryMaskingHandler(
        IReadOnlyList<string> sensitiveParams,
        ILogger logger,
        string clientName)
    {
        _sensitiveParams = sensitiveParams;
        _logger = logger;
        _clientName = clientName;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var start = Environment.TickCount64;
        HttpResponseMessage response;

        try
        {
            response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            var maskedUrl = MaskUrl(request.RequestUri);
            _logger.LogError(ex,
                "[{ClientName}] {Method} {Url} — network error after {ElapsedMs}ms",
                _clientName, request.Method.Method, maskedUrl, Environment.TickCount64 - start);
            throw;
        }

        var elapsed = Environment.TickCount64 - start;
        var safeUrl = MaskUrl(request.RequestUri);

        _logger.LogInformation(
            "[{ClientName}] {Method} {Url} → {StatusCode} in {ElapsedMs}ms",
            _clientName, request.Method.Method, safeUrl, (int)response.StatusCode, elapsed);

        return response;
    }

    internal string MaskUrl(Uri? uri)
    {
        if (uri is null) return "(null)";

        var uriString = uri.ToString();
        if (string.IsNullOrEmpty(uri.Query)) return uriString;

        var query = uri.Query.TrimStart('?');
        var parts = query.Split('&');
        var masked = parts.Select(part =>
        {
            var eqIndex = part.IndexOf('=', StringComparison.Ordinal);
            if (eqIndex < 0) return part;

            var name = part[..eqIndex];
            var isSensitive = _sensitiveParams.Any(p =>
                p.Equals(name, StringComparison.OrdinalIgnoreCase));

            return isSensitive ? $"{name}=***REDACTED***" : part;
        });

        var maskedQuery = string.Join("&", masked);
        return uriString.Replace(uri.Query.TrimStart('?'), maskedQuery, StringComparison.Ordinal);
    }
}

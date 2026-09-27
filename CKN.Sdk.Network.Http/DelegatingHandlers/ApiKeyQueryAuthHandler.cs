using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CKN.Sdk.Network.Http.DelegatingHandlers;

/// <summary>
/// Appends a fixed API key as a query-string parameter to every outgoing request.
/// </summary>
internal sealed class ApiKeyQueryAuthHandler : DelegatingHandler
{
    private readonly string _parameterName;
    private readonly string _value;

    internal ApiKeyQueryAuthHandler(string parameterName, string value)
    {
        _parameterName = parameterName;
        _value = value;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (request.RequestUri is not null)
        {
            var separator = string.IsNullOrEmpty(request.RequestUri.Query) ? "?" : "&";
            var newUri = new System.Uri(
                $"{request.RequestUri}{separator}{_parameterName}={_value}");
            request.RequestUri = newUri;
        }

        return base.SendAsync(request, cancellationToken);
    }
}

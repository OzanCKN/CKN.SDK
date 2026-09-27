using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CKN.Sdk.Network.Http.DelegatingHandlers;

/// <summary>
/// Adds a fixed API key as an HTTP request header to every outgoing request.
/// </summary>
internal sealed class ApiKeyHeaderAuthHandler : DelegatingHandler
{
    private readonly string _headerName;
    private readonly string _value;

    internal ApiKeyHeaderAuthHandler(string headerName, string value)
    {
        _headerName = headerName;
        _value = value;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        request.Headers.TryAddWithoutValidation(_headerName, _value);
        return base.SendAsync(request, cancellationToken);
    }
}

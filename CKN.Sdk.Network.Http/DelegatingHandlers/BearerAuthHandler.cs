using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace CKN.Sdk.Network.Http.DelegatingHandlers;

/// <summary>
/// Adds a Bearer token to the <c>Authorization</c> header of every outgoing request.
/// </summary>
internal sealed class BearerAuthHandler : DelegatingHandler
{
    private readonly string _token;

    internal BearerAuthHandler(string token)
    {
        _token = token;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        return base.SendAsync(request, cancellationToken);
    }
}

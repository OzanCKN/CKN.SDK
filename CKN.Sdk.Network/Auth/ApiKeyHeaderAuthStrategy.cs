namespace CKN.Sdk.Network.Auth;

/// <summary>
/// Sends the API key as a custom HTTP request header on every request.
/// </summary>
/// <remarks>
/// Example: <c>X-API-Key: abc123</c>
/// </remarks>
public sealed class ApiKeyHeaderAuthStrategy : CknAuthStrategy
{
    /// <summary>Gets the name of the HTTP header.</summary>
    public string HeaderName { get; }

    /// <summary>Gets the API key value.</summary>
    public string Value { get; }

    /// <summary>
    /// Initialises a new <see cref="ApiKeyHeaderAuthStrategy"/>.
    /// </summary>
    /// <param name="headerName">The HTTP header name, e.g. <c>X-API-Key</c>.</param>
    /// <param name="value">The secret API key value.</param>
    public ApiKeyHeaderAuthStrategy(string headerName, string value)
    {
        HeaderName = headerName;
        Value = value;
    }
}

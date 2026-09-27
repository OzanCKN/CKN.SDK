namespace CKN.Sdk.Network.Auth;

/// <summary>
/// Appends the API key as a query string parameter on every request.
/// The parameter name and value are automatically masked in structured logs.
/// </summary>
/// <remarks>
/// Example: <c>https://api.example.com/data?token=abc123</c>
/// </remarks>
public sealed class ApiKeyQueryAuthStrategy : CknAuthStrategy
{
    /// <summary>Gets the query string parameter name, e.g. <c>token</c> or <c>apikey</c>.</summary>
    public string ParameterName { get; }

    /// <summary>Gets the API key value.</summary>
    public string Value { get; }

    /// <summary>
    /// Initialises a new <see cref="ApiKeyQueryAuthStrategy"/>.
    /// </summary>
    /// <param name="parameterName">The query parameter name, e.g. <c>token</c>.</param>
    /// <param name="value">The secret API key value.</param>
    public ApiKeyQueryAuthStrategy(string parameterName, string value)
    {
        ParameterName = parameterName;
        Value = value;
    }
}

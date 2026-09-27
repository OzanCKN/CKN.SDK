namespace CKN.Sdk.Network.Auth;

/// <summary>
/// Sends a Bearer token in the <c>Authorization</c> header on every request.
/// </summary>
/// <remarks>
/// Example: <c>Authorization: Bearer eyJhbG...</c>
/// </remarks>
public sealed class BearerAuthStrategy : CknAuthStrategy
{
    /// <summary>Gets the Bearer token value (without the "Bearer " prefix).</summary>
    public string Token { get; }

    /// <summary>
    /// Initialises a new <see cref="BearerAuthStrategy"/>.
    /// </summary>
    /// <param name="token">The Bearer token string.</param>
    public BearerAuthStrategy(string token)
    {
        Token = token;
    }
}

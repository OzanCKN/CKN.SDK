namespace CKN.Sdk.Network.Auth;

/// <summary>
/// Base class for HTTP authentication strategies. Use one of the concrete subclasses:
/// <see cref="ApiKeyHeaderAuthStrategy"/>, <see cref="ApiKeyQueryAuthStrategy"/>, or <see cref="BearerAuthStrategy"/>.
/// </summary>
public abstract class CknAuthStrategy { }

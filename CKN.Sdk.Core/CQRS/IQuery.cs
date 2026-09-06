using MediatR;

namespace CKN.Sdk.Core.CQRS;

/// <summary>
/// Represents a CQRS Query that returns a response.
/// </summary>
public interface IQuery<out TResponse> : IRequest<TResponse>
{
}

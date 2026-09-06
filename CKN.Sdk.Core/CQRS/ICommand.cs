using MediatR;

namespace CKN.Sdk.Core.CQRS;

/// <summary>
/// Represents a CQRS Command.
/// </summary>
public interface ICommand : IRequest
{
}

/// <summary>
/// Represents a CQRS Command that returns a response.
/// </summary>
public interface ICommand<out TResponse> : IRequest<TResponse>
{
}

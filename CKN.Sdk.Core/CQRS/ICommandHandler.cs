using MediatR;

namespace CKN.Sdk.Core.CQRS;

/// <summary>
/// Represents a handler for a CQRS Command.
/// </summary>
public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand>
    where TCommand : ICommand
{
}

/// <summary>
/// Represents a handler for a CQRS Command that returns a response.
/// </summary>
public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
}

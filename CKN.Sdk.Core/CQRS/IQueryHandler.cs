using MediatR;

namespace CKN.Sdk.Core.CQRS;

/// <summary>
/// Represents a handler for a CQRS Query.
/// </summary>
public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
}

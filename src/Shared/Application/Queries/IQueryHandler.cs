using MediatR;

namespace ModularMonolith.Shared.Application.Queries;

/// <summary>
/// Handler interface for queries
/// </summary>
public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
}

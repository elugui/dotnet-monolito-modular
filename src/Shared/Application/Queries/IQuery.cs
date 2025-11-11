using MediatR;

namespace ModularMonolith.Shared.Application.Queries;

/// <summary>
/// Marker interface for queries
/// </summary>
public interface IQuery<out TResponse> : IRequest<TResponse>
{
}

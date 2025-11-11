using MediatR;

namespace ModularMonolith.Shared.Application.Commands;

/// <summary>
/// Marker interface for commands that return a result
/// </summary>
public interface ICommand<out TResponse> : IRequest<TResponse>
{
}

/// <summary>
/// Marker interface for commands without result
/// </summary>
public interface ICommand : IRequest
{
}

using MediatR;

namespace ModularMonolith.Shared.Application.Commands;

/// <summary>
/// Handler interface for commands that return a result
/// </summary>
public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
}

/// <summary>
/// Handler interface for commands without result
/// </summary>
public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand>
    where TCommand : ICommand
{
}

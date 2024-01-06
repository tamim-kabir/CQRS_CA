using Domain.Shared;
using MediatR;

namespace Application.Abstractions.Messaging.Command;
public interface ICommandHandler<TCommend> : IRequestHandler<TCommend, Result>
    where TCommend : ICommand
{
}
public interface ICommandHandler<TCommend, TResponse> : IRequestHandler<TCommend, Result<TResponse>>
    where TCommend : ICommand<TResponse>
{
}

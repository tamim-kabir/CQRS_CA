using Domain.Shared;
using MediatR;

namespace Application.Abstractions.Messaging.Command;
public interface ICommand : IRequest<Result>
{
}
public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}


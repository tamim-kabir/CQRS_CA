using Domain.Shared;
using MediatR;

namespace Application.Abstractions.Messaging.Query;
public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}

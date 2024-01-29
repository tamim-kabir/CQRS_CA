using Domain.Shared;
using MediatR;

namespace Application.Abstractions.Messaging.Query;
public interface IQueryHandler<TQuery, TResponse>
    : IRequestHandler<TQuery, Result<TResponse>> where TQuery : IQuery<TResponse>
{
}

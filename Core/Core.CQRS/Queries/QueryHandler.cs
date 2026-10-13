using MediatR;

namespace Core.CQRS.Queries;

public abstract class QueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    public abstract Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken);
}

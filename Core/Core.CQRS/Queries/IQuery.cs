using MediatR;

namespace Core.CQRS.Queries;

public interface IQuery<out TResponse> : IRequest<TResponse>
{
}

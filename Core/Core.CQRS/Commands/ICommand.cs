using MediatR;

namespace Core.CQRS.Commands;

public interface ICommand<out TResponse> : IRequest<TResponse>
{
}

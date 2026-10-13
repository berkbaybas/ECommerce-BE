using MediatR;

namespace Core.CQRS.Processing;

public interface ICQRSProcessor
{
    Task<TResponse> ProcessAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken);
}

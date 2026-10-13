using MediatR;

namespace Core.CQRS.Processing;

internal sealed class CQRSProcessor(IMediator mediator) : ICQRSProcessor
{
    public Task<TResponse> ProcessAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken)
        => mediator.Send(request, cancellationToken);
}

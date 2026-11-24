using BFF.Application.RemoteCall;
using Core.CQRS.Commands;
using Core.CQRS.Queries;
using Product.Abstraction.Requests.Products;

namespace BFF.Application.Products;

public sealed record ListProductsQuery : IQuery<RemoteResponse>;
public sealed record SearchProductsQuery(string QueryString) : IQuery<RemoteResponse>;
public sealed record FindProductQuery(string Id) : IQuery<RemoteResponse>;
public sealed record CreateProductCommand(CreateProductRequest Request) : ICommand<RemoteResponse>;
public sealed record UpdateProductDetailsCommand(string Id, UpdateProductDetailsRequest Request) : ICommand<RemoteResponse>;
public sealed record UpdateProductStockCommand(string Id, UpdateProductStockRequest Request) : ICommand<RemoteResponse>;
public sealed record UpdateProductPriceCommand(string Id, UpdateProductPriceRequest Request) : ICommand<RemoteResponse>;
public sealed record DeleteProductCommand(string Id) : ICommand<RemoteResponse>;

public sealed class ListProductsQueryHandler(IProductRemoteCallService client) : QueryHandler<ListProductsQuery, RemoteResponse>
{ public override Task<RemoteResponse> Handle(ListProductsQuery query, CancellationToken ct) => client.List(ct); }
public sealed class SearchProductsQueryHandler(IProductRemoteCallService client) : QueryHandler<SearchProductsQuery, RemoteResponse>
{ public override Task<RemoteResponse> Handle(SearchProductsQuery query, CancellationToken ct) => client.Search(query.QueryString, ct); }
public sealed class FindProductQueryHandler(IProductRemoteCallService client) : QueryHandler<FindProductQuery, RemoteResponse>
{ public override Task<RemoteResponse> Handle(FindProductQuery query, CancellationToken ct) => client.Find(query.Id, ct); }
public sealed class CreateProductCommandHandler(IProductRemoteCallService client) : CommandHandler<CreateProductCommand, RemoteResponse>
{ public override Task<RemoteResponse> Handle(CreateProductCommand command, CancellationToken ct) => client.Create(command.Request, ct); }
public sealed class UpdateProductDetailsCommandHandler(IProductRemoteCallService client) : CommandHandler<UpdateProductDetailsCommand, RemoteResponse>
{ public override Task<RemoteResponse> Handle(UpdateProductDetailsCommand command, CancellationToken ct) => client.UpdateDetails(command.Id, command.Request, ct); }
public sealed class UpdateProductStockCommandHandler(IProductRemoteCallService client) : CommandHandler<UpdateProductStockCommand, RemoteResponse>
{ public override Task<RemoteResponse> Handle(UpdateProductStockCommand command, CancellationToken ct) => client.UpdateStock(command.Id, command.Request, ct); }
public sealed class UpdateProductPriceCommandHandler(IProductRemoteCallService client) : CommandHandler<UpdateProductPriceCommand, RemoteResponse>
{ public override Task<RemoteResponse> Handle(UpdateProductPriceCommand command, CancellationToken ct) => client.UpdatePrice(command.Id, command.Request, ct); }
public sealed class DeleteProductCommandHandler(IProductRemoteCallService client) : CommandHandler<DeleteProductCommand, RemoteResponse>
{ public override Task<RemoteResponse> Handle(DeleteProductCommand command, CancellationToken ct) => client.Delete(command.Id, ct); }

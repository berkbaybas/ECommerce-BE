using BFF.Application.RemoteCall;
using Category.Abstraction;
using Core.CQRS.Commands;
using Core.CQRS.Queries;

namespace BFF.Application.Categories;

public sealed record ListCategoriesQuery : IQuery<RemoteResponse>;
public sealed record FindCategoryQuery(string Id) : IQuery<RemoteResponse>;
public sealed record CreateCategoryCommand(CategoryRequest Request) : ICommand<RemoteResponse>;
public sealed record UpdateCategoryCommand(string Id, CategoryRequest Request) : ICommand<RemoteResponse>;
public sealed record DeleteCategoryCommand(string Id) : ICommand<RemoteResponse>;

public sealed class ListCategoriesQueryHandler(ICategoryRemoteCallService client) : QueryHandler<ListCategoriesQuery, RemoteResponse>
{ public override Task<RemoteResponse> Handle(ListCategoriesQuery query, CancellationToken ct) => client.List(ct); }
public sealed class FindCategoryQueryHandler(ICategoryRemoteCallService client) : QueryHandler<FindCategoryQuery, RemoteResponse>
{ public override Task<RemoteResponse> Handle(FindCategoryQuery query, CancellationToken ct) => client.Find(query.Id, ct); }
public sealed class CreateCategoryCommandHandler(ICategoryRemoteCallService client) : CommandHandler<CreateCategoryCommand, RemoteResponse>
{ public override Task<RemoteResponse> Handle(CreateCategoryCommand command, CancellationToken ct) => client.Create(command.Request, ct); }
public sealed class UpdateCategoryCommandHandler(ICategoryRemoteCallService client) : CommandHandler<UpdateCategoryCommand, RemoteResponse>
{ public override Task<RemoteResponse> Handle(UpdateCategoryCommand command, CancellationToken ct) => client.Update(command.Id, command.Request, ct); }
public sealed class DeleteCategoryCommandHandler(ICategoryRemoteCallService client) : CommandHandler<DeleteCategoryCommand, RemoteResponse>
{ public override Task<RemoteResponse> Handle(DeleteCategoryCommand command, CancellationToken ct) => client.Delete(command.Id, ct); }

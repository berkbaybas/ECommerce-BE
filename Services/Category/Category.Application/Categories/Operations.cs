using Core.Abstraction.Exceptions;
using Core.CQRS.Commands;
using Core.CQRS.Queries;
using Core.Validation.Validators;
using FluentValidation;
using MediatR;
using Category.Abstraction;
using Category.Core.Exceptions;
using Category.Infrastructure.Repositories;
using Entity = Category.Domain.Entities;
namespace Category.Application.Categories;

public sealed record CreateCategoryCommand(string Name, string? Description) : ICommand<CategoryDto>;
public sealed record UpdateCategoryCommand(string Id, string Name, string? Description) : ICommand<Unit>;
public sealed record DeleteCategoryCommand(string Id) : ICommand<Unit>;
public sealed record FindCategoryQuery(string Id) : IQuery<CategoryDto>;
public sealed record ListCategoriesQuery : IQuery<IList<CategoryDto>>;
public sealed class CreateCategoryCommandValidator : BaseValidator<CreateCategoryCommand>
{ public CreateCategoryCommandValidator() => this.RuleFor(x => x.Name).NotEmpty().WithMessage(CategoryLocalizations.Errors.Validation.NameEmpty); }
public sealed class UpdateCategoryCommandValidator : BaseValidator<UpdateCategoryCommand>
{ public UpdateCategoryCommandValidator() => this.RuleFor(x => x.Name).NotEmpty().WithMessage(CategoryLocalizations.Errors.Validation.NameEmpty); }
internal static class CategoryMapping
{ public static CategoryDto ToDto(this Entity.Category value) => new() { Id = value.Id, Name = value.Name, Description = value.Description, CreatedAt = value.CreatedAt }; }
public sealed class CreateCategoryCommandHandler(ICategoryRepository repository) : CommandHandler<CreateCategoryCommand, CategoryDto>
{
    public override async Task<CategoryDto> Handle(CreateCategoryCommand command, CancellationToken cancellationToken)
    { var category = new Entity.Category(command.Name, command.Description); await repository.AddAsync(category, cancellationToken); return category.ToDto(); }
}
public sealed class UpdateCategoryCommandHandler(ICategoryRepository repository) : CommandHandler<UpdateCategoryCommand, Unit>
{
    public override async Task<Unit> Handle(UpdateCategoryCommand command, CancellationToken cancellationToken)
    { var category = await repository.FindAsync(x => x.Id == command.Id, cancellationToken); NotFoundException.ThrowIfNull(category, CategoryLocalizations.Errors.Business.NotFound); category!.UpdateDetails(command.Name, command.Description); await repository.UpdateAsync(category, cancellationToken); return Unit.Value; }
}
public sealed class DeleteCategoryCommandHandler(ICategoryRepository repository) : CommandHandler<DeleteCategoryCommand, Unit>
{
    public override async Task<Unit> Handle(DeleteCategoryCommand command, CancellationToken cancellationToken)
    { var exists = await repository.ExistsAsync(x => x.Id == command.Id, cancellationToken); NotFoundException.ThrowIf(!exists, CategoryLocalizations.Errors.Business.NotFound); await repository.DeleteAsync(command.Id, cancellationToken); return Unit.Value; }
}
public sealed class FindCategoryQueryHandler(ICategoryRepository repository) : QueryHandler<FindCategoryQuery, CategoryDto>
{
    public override async Task<CategoryDto> Handle(FindCategoryQuery query, CancellationToken cancellationToken)
    { var category = await repository.FindAsync(x => x.Id == query.Id, cancellationToken); NotFoundException.ThrowIfNull(category, CategoryLocalizations.Errors.Business.NotFound); return category!.ToDto(); }
}
public sealed class ListCategoriesQueryHandler(ICategoryRepository repository) : QueryHandler<ListCategoriesQuery, IList<CategoryDto>>
{
    public override async Task<IList<CategoryDto>> Handle(ListCategoriesQuery query, CancellationToken cancellationToken)
    { var categories = await repository.ListAsync(null, cancellationToken); return categories.Select(x => x.ToDto()).ToList(); }
}

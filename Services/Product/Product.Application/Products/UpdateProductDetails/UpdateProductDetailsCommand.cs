using Core.CQRS.Commands;
using MediatR;

namespace Product.Application.Products;

public sealed class UpdateProductDetailsCommand(string id, string name, string? description) : ICommand<Unit>
{
    public string Id { get; } = id;

    public string Name { get; } = name;

    public string? Description { get; } = description;
}

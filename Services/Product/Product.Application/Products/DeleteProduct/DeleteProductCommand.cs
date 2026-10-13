using Core.CQRS.Commands;
using MediatR;

namespace Product.Application.Products;

public sealed class DeleteProductCommand(string id) : ICommand<Unit>
{
    public string Id { get; } = id;
}

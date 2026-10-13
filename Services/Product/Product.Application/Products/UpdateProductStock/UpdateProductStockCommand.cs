using Core.CQRS.Commands;
using Product.Abstraction.Responses.Products;

namespace Product.Application.Products;

public sealed class UpdateProductStockCommand(string id, int stock) : ICommand<ProductDto>
{
    public string Id { get; } = id;

    public int Stock { get; } = stock;
}

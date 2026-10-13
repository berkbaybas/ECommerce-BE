using Core.CQRS.Commands;
using Product.Abstraction.Responses.Products;

namespace Product.Application.Products;

public sealed class UpdateProductPriceCommand(string id, decimal price) : ICommand<ProductDto>
{
    public string Id { get; } = id;

    public decimal Price { get; } = price;
}

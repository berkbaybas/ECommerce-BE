using Core.CQRS.Commands;
using Product.Abstraction.Responses.Products;

namespace Product.Application.Products;

public sealed class CreateProductCommand(string name, string sku, string? description, decimal price, int stock)
    : ICommand<ProductDto>
{
    public string Name { get; } = name;

    public string Sku { get; } = sku;

    public string? Description { get; } = description;

    public decimal Price { get; } = price;

    public int Stock { get; } = stock;
}

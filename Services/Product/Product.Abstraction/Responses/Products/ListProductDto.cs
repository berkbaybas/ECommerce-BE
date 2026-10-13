using Product.Abstraction.Enums;

namespace Product.Abstraction.Responses.Products;

public sealed class ListProductDto
{
    public string Id { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public string Sku { get; init; } = string.Empty;

    public decimal Price { get; init; }

    public int Stock { get; init; }

    public ProductStatus Status { get; init; }
}

using Product.Abstraction.Enums;

namespace Product.Abstraction.Responses.Products;

public sealed class ProductDto
{
    public string Id { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public string Sku { get; init; } = string.Empty;

    public string? Description { get; init; }

    public decimal Price { get; init; }

    public int Stock { get; init; }

    public ProductStatus Status { get; init; }

    public DateTime CreatedAt { get; init; }
}

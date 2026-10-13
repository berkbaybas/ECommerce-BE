namespace Product.Abstraction.Requests.Products;

public sealed class UpdateProductDetailsRequest
{
    public string Name { get; init; } = string.Empty;

    public string? Description { get; init; }
}

namespace Product.Abstraction.Responses.Products;

public sealed class ProductPriceChangedPayload
{
    public string ProductId { get; init; } = string.Empty;

    public decimal Price { get; init; }
}

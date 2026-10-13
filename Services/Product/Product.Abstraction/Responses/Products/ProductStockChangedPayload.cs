namespace Product.Abstraction.Responses.Products;

public sealed class ProductStockChangedPayload
{
    public string ProductId { get; init; } = string.Empty;

    public int Stock { get; init; }
}

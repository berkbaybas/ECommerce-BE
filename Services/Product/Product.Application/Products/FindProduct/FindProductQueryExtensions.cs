using Product.Abstraction.Responses.Products;
using Entity = Product.Domain.Entities;

namespace Product.Application.Products.FindProduct;

internal static class FindProductQueryExtensions
{
    public static ProductDto ToMapResponse(this Entity.Product product) => new()
    {
        Id = product.Id,
        Name = product.Name,
        Sku = product.Sku,
        Description = product.Description,
        Price = product.Price,
        Stock = product.Stock,
        Status = product.Status,
        CreatedAt = product.CreatedAt,
    };
}

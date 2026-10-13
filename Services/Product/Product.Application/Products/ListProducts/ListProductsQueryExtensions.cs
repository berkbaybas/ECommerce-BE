using Product.Abstraction.Responses.Products;
using Entity = Product.Domain.Entities;

namespace Product.Application.Products.ListProducts;

internal static class ListProductsQueryExtensions
{
    public static ListProductDto ToMapResponse(this Entity.Product product) => new()
    {
        Id = product.Id,
        Name = product.Name,
        Sku = product.Sku,
        Price = product.Price,
        Stock = product.Stock,
        Status = product.Status,
    };
}

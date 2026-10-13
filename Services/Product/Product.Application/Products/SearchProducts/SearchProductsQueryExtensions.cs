using Core.Abstraction.Paging;
using Product.Abstraction.Responses.Products;
using Entity = Product.Domain.Entities;

namespace Product.Application.Products.SearchProducts;

internal static class SearchProductsQueryExtensions
{
    public static PagedResult<ListProductDto> ToMapResponse(this PagedResult<Entity.Product> result) => new()
    {
        Items = result.Items.Select(x => x.ToMapResponse()).ToList(),
        Page = result.Page,
        PageSize = result.PageSize,
        TotalCount = result.TotalCount,
    };

    private static ListProductDto ToMapResponse(this Entity.Product product) => new()
    {
        Id = product.Id,
        Name = product.Name,
        Sku = product.Sku,
        Price = product.Price,
        Stock = product.Stock,
        Status = product.Status,
    };
}

using Category.Abstraction;
using Core.Abstraction.Responses;
using Product.Abstraction.Responses.Products;
namespace BFF.API;
public sealed class CatalogResponse
{
    public BaseResponse<IList<ListProductDto>>? Products { get; init; }
    public BaseResponse<IList<CategoryDto>>? Categories { get; init; }
}

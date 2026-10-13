using Core.CQRS.Queries;
using Product.Abstraction.Responses.Products;

namespace Product.Application.Products;

public sealed class ListProductsQuery : IQuery<IList<ListProductDto>>
{
}

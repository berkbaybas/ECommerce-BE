using Core.CQRS.Queries;
using Product.Abstraction.Responses.Products;

namespace Product.Application.Products;

public sealed class FindProductQuery(string id) : IQuery<ProductDto>
{
    public string Id { get; } = id;
}

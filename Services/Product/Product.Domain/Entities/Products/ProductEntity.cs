using Core.Abstraction.Entities;
using Product.Abstraction.Enums;

namespace Product.Domain.Entities;

public sealed partial class Product : BaseEntity
{
    public Product()
    {
    }

    public string Name { get; protected set; } = string.Empty;

    public string Sku { get; protected set; } = string.Empty;

    public string? Description { get; protected set; }

    public decimal Price { get; protected set; }

    public int Stock { get; protected set; }

    public ProductStatus Status { get; protected set; }
}

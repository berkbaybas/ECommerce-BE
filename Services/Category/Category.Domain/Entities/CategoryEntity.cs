using Core.Abstraction.Entities;
namespace Category.Domain.Entities;
public sealed partial class Category : BaseEntity
{
    public Category() { }
    public string Name { get; protected set; } = string.Empty;
    public string? Description { get; protected set; }
}

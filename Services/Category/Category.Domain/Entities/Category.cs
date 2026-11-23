using Core.Abstraction.Exceptions;
using Category.Core.Exceptions;
namespace Category.Domain.Entities;
public sealed partial class Category
{
    public Category(string name, string? description)
    {
        BusinessException.ThrowIfNullOrWhiteSpace(name, CategoryLocalizations.Errors.Validation.NameEmpty);
        this.Name = name;
        this.Description = description;
    }
    public void UpdateDetails(string name, string? description)
    {
        BusinessException.ThrowIfNullOrWhiteSpace(name, CategoryLocalizations.Errors.Validation.NameEmpty);
        this.Name = name; this.Description = description; this.MarkUpdated();
    }
}

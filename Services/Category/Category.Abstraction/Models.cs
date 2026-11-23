namespace Category.Abstraction;
public sealed class CategoryRequest { public string Name { get; init; } = string.Empty; public string? Description { get; init; } }
public sealed class CategoryDto { public string Id { get; init; } = string.Empty; public string Name { get; init; } = string.Empty; public string? Description { get; init; } public DateTime CreatedAt { get; init; } }

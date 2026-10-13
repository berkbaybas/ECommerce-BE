namespace Core.Abstraction.Entities;

public abstract class BaseEntity
{
    protected BaseEntity()
    {
        this.Id = Guid.NewGuid().ToString("N");
        this.CreatedAt = DateTime.UtcNow;
    }

    public string Id { get; protected set; }

    public DateTime CreatedAt { get; protected set; }

    public DateTime? UpdatedAt { get; protected set; }

    public bool IsDeleted { get; protected set; }

    public void MarkUpdated() => this.UpdatedAt = DateTime.UtcNow;

    public void MarkDeleted()
    {
        this.IsDeleted = true;
        this.MarkUpdated();
    }
}

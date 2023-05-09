namespace Theatre.Domain.Abstractions;

public abstract class Entity
{
    protected Entity()
    {}
    public Guid Id { get; protected set; }
}
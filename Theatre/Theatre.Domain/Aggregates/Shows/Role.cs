using Theatre.Domain.Abstractions;

namespace Theatre.Domain.Aggregates.Shows;

public class Role : Entity
{
    public Guid ShowId { get; private set; }
    public string Title { get; private set; }

    public Role(Guid id, Guid showId, string title)
    {
        Id = id;
        ShowId = showId;
        Title = title;
    }
}
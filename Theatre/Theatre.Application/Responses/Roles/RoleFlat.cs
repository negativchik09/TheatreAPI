namespace Theatre.Application.Responses.Roles;

public record RoleFlat
{
    public Guid Id { get; set; }
    public string Title { get; set; }
}
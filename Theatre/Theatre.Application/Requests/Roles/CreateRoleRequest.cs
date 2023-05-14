namespace Theatre.Application.Requests.Roles;

public record CreateRoleRequest
{
    public Guid ShowId { get; set; }
    public string Title { get; set; }
}
namespace Theatre.Application.Requests.Roles;

public record CreateRoleRequest(Guid ShowId, string Title);
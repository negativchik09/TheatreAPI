using Theatre.Application.Requests.Contracts;
using Theatre.Application.Responses.Actors;

namespace Theatre.Application.Responses.Roles;

public record RoleDescription
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public ActorFlat? Actor { get; set; }
    public ContractFlat? Contract { get; set; }
}
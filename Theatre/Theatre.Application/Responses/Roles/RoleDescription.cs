using Theatre.Application.Requests.Contracts;
using Theatre.Application.Responses.Actors;

namespace Theatre.Application.Responses.Roles;

public record RoleDescription(
    Guid Id,
    string Title,
    ActorFlat? Actor,
    ContractFlat? Contract);
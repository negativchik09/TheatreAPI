namespace Theatre.Application.Requests.Contracts;

public record ContractFlat(
    Guid Id,
    Guid ShowId, 
    Guid RoleId, 
    Guid ActorId, 
    double Sum);
namespace Theatre.Application.Requests.Contracts;

public record CreateContractRequest(
    Guid ShowId, 
    Guid RoleId, 
    Guid ActorId, 
    double Sum);
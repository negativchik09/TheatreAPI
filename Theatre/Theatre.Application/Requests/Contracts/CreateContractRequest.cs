namespace Theatre.Application.Requests.Contracts;

public record CreateContractRequest
{
    public Guid ShowId { get; set; }
    public Guid RoleId { get; set; }
    public Guid ActorId { get; set; }
    public double Sum { get; set; }
}
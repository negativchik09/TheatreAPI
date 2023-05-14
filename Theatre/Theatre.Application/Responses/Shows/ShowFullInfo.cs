using Theatre.Application.Responses.Roles;

namespace Theatre.Application.Responses.Shows;

public record ShowFullInfo
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public double TotalBudget { get; set; }
    public double AlreadySpent { get; set; }
    public DateTime DateOfPremiere { get; set; }
    public IEnumerable<RoleDescription> RoleActorPairs { get; set; }
}
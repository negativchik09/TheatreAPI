using Theatre.Application.Responses.Roles;

namespace Theatre.Application.Responses.Shows;

public record ShowFullInfo(
    Guid Id,
    string Title,
    double TotalBudget,
    double AlreadySpent,
    DateTime DateOfPremiere,
    IEnumerable<RoleDescription> RoleActorPairs);
namespace Theatre.Application.Responses.Shows;

public record ShowTableView
(
    Guid Id, 
    string Title, 
    double TotalBudget, 
    double AlreadySpent,
    DateTime DateOfPremiere,
    int RoleCount,
    int ActorsCount
);
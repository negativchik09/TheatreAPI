namespace Theatre.Application.Requests.Shows;

public record CreateShowRequest(string Title,
    double TotalBudget,
    DateTime DateOfPremiere);
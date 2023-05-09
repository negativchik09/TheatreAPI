namespace Theatre.Application.Responses.Shows;

public record ShowFlat(Guid Id, 
    string Title, 
    double TotalBudget,
    DateTime DateOfPremiere);
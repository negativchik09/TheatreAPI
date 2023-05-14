namespace Theatre.Application.Responses.Shows;

public record ShowTableView
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public double TotalBudget { get; set; }
    public double AlreadySpent { get; set; }
    public DateTime DateOfPremiere { get; set; }
    public int RoleCount { get; set; }
    public int ActorsCount { get; set; }
}
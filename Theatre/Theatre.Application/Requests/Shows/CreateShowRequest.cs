namespace Theatre.Application.Requests.Shows;

public record CreateShowRequest
{
    public string Title { get; set; }
    public double TotalBudget { get; set; }
    public DateTime DateOfPremiere { get; set; }
}
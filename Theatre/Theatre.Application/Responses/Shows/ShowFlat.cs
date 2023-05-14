namespace Theatre.Application.Responses.Shows;

public record ShowFlat
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public double TotalBudget { get; set; }
    public DateTime DateOfPremiere { get; set; }
}
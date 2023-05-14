namespace Theatre.Application.Responses.Actors;

public record CreateActorResponse
{
    public string Login { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MiddleName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Dignity { get; set; }
    public double Experience { get; set; }
    public string Email { get; set; }
    public string Telephone { get; set; }
    public string Address { get; set; }
    public string PassportNumber { get; set; }
    public string PassportGivenBy { get; set; }
    public string? PassportSeries { get; set; }
    public string TaxesNumber { get; set; }
}
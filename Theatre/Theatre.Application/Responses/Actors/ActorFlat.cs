namespace Theatre.Application.Responses.Actors;

public record ActorFlat
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MiddleName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Dignity { get; set; }
    public double Experience { get; set; }
}
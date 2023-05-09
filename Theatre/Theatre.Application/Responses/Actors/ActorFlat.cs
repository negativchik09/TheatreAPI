namespace Theatre.Application.Responses.Actors;

public record ActorFlat
(
    Guid Id,
    string FirstName, 
    string LastName, 
    string MiddleName, 
    DateTime DateOfBirth,
    string Dignity, 
    double Experience
);
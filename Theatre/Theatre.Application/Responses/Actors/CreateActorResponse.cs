namespace Theatre.Application.Responses.Actors;

public record CreateActorResponse
(
    string FirstName, 
    string LastName, 
    string MiddleName, 
    DateTime DateOfBirth,
    string Dignity, 
    double Experience,
    string Login,
    string Password
);
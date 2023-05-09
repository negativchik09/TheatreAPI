namespace Theatre.Application.Requests.Actors;

public record UpdatePasswordRequest(
    string OldPassword, 
    string NewPassword);
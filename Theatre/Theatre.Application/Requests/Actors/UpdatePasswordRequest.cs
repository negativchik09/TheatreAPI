namespace Theatre.Application.Requests.Actors;

public record UpdatePasswordRequest
{
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
}
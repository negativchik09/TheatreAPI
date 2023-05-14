namespace Theatre.Application.Requests.Account;

public record LoginRequest
{
    public string Login { get; set; }
    public string Password { get; set; }
}
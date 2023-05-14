namespace Theatre.Application.Responses.Account;

public record LoginResponse
{
    public string Token { get; set; }
    public string Role { get; set; }
}
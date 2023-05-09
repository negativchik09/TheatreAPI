using Microsoft.AspNetCore.Identity;
using Theatre.Application.Requests.Account;
using Theatre.Application.Requests.Actors;
using Theatre.Application.Responses.Account;
using Theatre.Errors;

namespace Theatre.Application.Abstractions;

public interface IAccountService
{
    Task<Result<LoginResponse>> Login(LoginRequest request);
    Task<Result<(IdentityUser User, string Password)>> CreateActor(string login, string email, string phone);
    Task<Result> UpdatePassword(UpdatePasswordRequest request, string userId);
    Task<Result> DeleteUser(Guid userId);
}
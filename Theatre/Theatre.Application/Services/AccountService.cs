using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Theatre.Application.Abstractions;
using Theatre.Application.Requests.Account;
using Theatre.Application.Requests.Actors;
using Theatre.Application.Responses.Account;
using Theatre.Errors;

namespace Theatre.Application.Services;

public class AccountService : IAccountService
{
    private UserManager<IdentityUser> _userManager;
    private SignInManager<IdentityUser> _signInManager;
    private IConfiguration _configuration;

    public AccountService(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }

    public async Task<Result<LoginResponse>> Login(LoginRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.Login);
        
        if (user == null) return Result.Failure<LoginResponse>(DefinedErrors.Accounts.UserNotFound);

        var result = await _signInManager.PasswordSignInAsync(
            user: user, 
            password: request.Password, 
            isPersistent: false, 
            lockoutOnFailure: false);

        if (!result.Succeeded)
        {
            return Result.Failure<LoginResponse>(DefinedErrors.Accounts.Identity);
        }
        
        var userRoles = await _userManager.GetRolesAsync(user);

        var authClaims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Sid, user.Id),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };
        
        authClaims.AddRange(userRoles.Select(userRole => new Claim(ClaimTypes.Role, userRole)));

        var token = GetToken(authClaims);
        
        return new LoginResponse(new JwtSecurityTokenHandler().WriteToken(token), userRoles.Single());
    }

    public async Task<Result<(IdentityUser User, string Password)>> CreateActor(string login, string email, string phone)
    {
        var user = new IdentityUser(login)
        {
            Email = email,
            PhoneNumber = phone
        };
        var password = GeneratePassword();
        var identityResult = await _userManager.CreateAsync(
            user, GeneratePassword());

        if (!identityResult.Succeeded)
        {
            return Result.Failure<(IdentityUser User, string Password)>(new Error(string.Join('\n', identityResult.Errors)));
        }

        await _userManager.AddToRoleAsync(user, "Actor");

        return (user, password);
    }

    public async Task<Result> UpdatePassword(UpdatePasswordRequest request, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        var identityResult = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);

        if (!identityResult.Succeeded)
        {
            return Result.Failure(new Error(string.Join('\n', identityResult.Errors)));
        }
        
        return Result.Success();
    }

    public async Task<Result> DeleteUser(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user != null)
        {
            await _userManager.DeleteAsync(user);
        }
        
        return Result.Success();
    }

    static string GeneratePassword(int length = 8)
    {
        string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        Random random = new Random();
        char[] password = new char[length];

        for (int i = 0; i < length; i++)
        {
            password[i] = characters[random.Next(characters.Length)];
        }

        return new string(password);
    }
    
    private JwtSecurityToken GetToken(IEnumerable<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            expires: DateTime.Now.AddHours(10000),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return token;
    }
}
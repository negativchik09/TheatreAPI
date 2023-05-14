using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Theatre.Application.Abstractions;
using Theatre.Application.Requests.Account;
using Theatre.Application.Requests.Actors;
using Theatre.Application.Responses.Account;
using Theatre.Errors;

namespace Theatre.Controllers;

[ApiController]
[Route("api/[controller]/")]
public class AccountController : Controller
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _accountService.Login(request);
        
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        if (result.Error == DefinedErrors.Accounts.UserNotFound)
        {
            return NotFound(result.Error.Message);
        }
        
        if (result.Error == DefinedErrors.Accounts.Identity)
        {
            return BadRequest(result.Error.Message);
        }

        return StatusCode(500, result.Error.Message);
    }
    
    [HttpPut]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequest request)
    {
        Claim? claim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid);
        var userId = claim?.Value;

        var result = await _accountService.UpdatePassword(request, userId!);
        
        if (result.IsSuccess)
        {
            return Ok();
        }

        return BadRequest(result.Error.Message);
    }
}
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Theatre.Application.Abstractions;
using Theatre.Application.Requests.Roles;
using Theatre.Application.Requests.Shows;
using Theatre.Application.Responses.Roles;
using Theatre.Application.Responses.Shows;
using Theatre.Errors;

namespace Theatre.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ShowController : Controller
{
    private readonly IShowService _showService;

    public ShowController(IShowService showService)
    {
        _showService = showService;
    }
    
    [HttpGet("")]
    [ProducesResponseType(typeof(IEnumerable<ShowTableView>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllForActor()
    {
        Claim? idClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid);
        Claim? roleClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role);

        if (idClaim is null) return StatusCode(500);
        if (roleClaim is null) return StatusCode(500);

        switch (roleClaim.Value)
        {
            case IdentityRoles.Actor:
            {
                var result = await _showService.GetByActor(Guid.Parse(idClaim.Value));
        
                if (result.IsSuccess)
                {
                    return Ok(result.Value);
                }

                if (result.Error == DefinedErrors.Actors.ActorNotFound)
                {
                    return NotFound(result.Error.Message);
                }
            
                return StatusCode(500, result.Error.Message);
            }
            case IdentityRoles.Admin:
            {
                var result = await _showService.GetAllFlat();

                if (result.IsSuccess)
                {
                    return Ok(result.Value);
                }

                return StatusCode(500, result.Error.Message);
            }
            default:
                return StatusCode(500);
        }
    }
    

    [HttpGet("{showId:guid}")]
    [ProducesResponseType(typeof(ShowFullInfo), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById([FromRoute] Guid showId)
    {
        var result = await _showService.GetById(showId);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        if (result.Error != DefinedErrors.Shows.ShowNotFound)
        {
            return NotFound(result.Error.Message);
        }

        return StatusCode(500, result.Error.Message);
    }
    
    [Authorize(Roles = IdentityRoles.Admin)]
    [HttpPost]
    [ProducesResponseType(typeof(ShowTableView), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateShow([FromBody] CreateShowRequest request)
    {
        var result = await _showService.CreateShow(request);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return BadRequest(result.Error.Message);
    }

    [Authorize(Roles = IdentityRoles.Admin)]
    [HttpDelete("{showId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteShow([FromRoute] Guid showId)
    {
        var result = await _showService.DeleteShow(showId);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return StatusCode(500, result.Error.Message);
    }
    
    [Authorize(Roles = IdentityRoles.Admin)]
    [HttpPost("create-role")]
    [ProducesResponseType(typeof(RoleDescription), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
    {
        var result = await _showService.CreateRole(request);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        if (result.Error == DefinedErrors.Shows.ShowNotFound)
        {
            return NotFound(result.Error.Message);
        }
        
        if (result.Error == DefinedErrors.Roles.RoleAlreadyCreatedForShow)
        {
            return BadRequest(result.Error.Message);
        }

        return StatusCode(500, result.Error.Message);
    }

    [Authorize(Roles = IdentityRoles.Admin)]
    [HttpDelete("delete-role/{roleId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteRole([FromRoute] Guid roleId)
    {
        var result = await _showService.DeleteRole(roleId);
        
        if (result.IsSuccess)
        {
            return Ok();
        }

        return StatusCode(500, result.Error.Message);
    }
}
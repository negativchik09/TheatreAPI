using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Theatre.Application.Abstractions;
using Theatre.Application.Requests.Roles;
using Theatre.Application.Requests.Shows;
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

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ShowTableView>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll()
    {
        Claim? roleClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role);
        Claim? idClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid);
        
        if (roleClaim is null) return StatusCode(500);
        if (idClaim is null) return StatusCode(500);

        Result<IEnumerable<ShowTableView>> result;

        if (roleClaim.Value == IdentityRoles.Admin)
        {
            result = await _showService.GetAllFlat();
        }
        else
        {
            result = await _showService.GetByActor(Guid.Parse(idClaim.Value));
        }

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        if (result.Error == DefinedErrors.Actors.ActorNotFound)
        {
            return NotFound(result.Error.Message);
        }

        return StatusCode(500, result.Error.Message);
    }
    
    [HttpGet("{showId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById([FromRoute] Guid showId)
    {
        var result = await _showService.GetById(showId);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        if (result.Error != DefinedErrors.Shows.ShowNotFound)
        {
            return NotFound(result.Error.Message);
        }

        return StatusCode(500, result.Error.Message);
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateShow([FromBody] CreateShowRequest request)
    {
        var result = await _showService.CreateShow(request);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return BadRequest(result.Error.Message);
    }

    [HttpDelete("{showId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
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
    
    [HttpPost("{showId:guid}/create-role")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request, [FromRoute] Guid showId)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("delete-role/{roleId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteRole([FromRoute] Guid roleId)
    {
        throw new NotImplementedException();
    }
}
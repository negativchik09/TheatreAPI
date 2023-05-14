using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Theatre.Application.Abstractions;
using Theatre.Application.Requests.Actors;
using Theatre.Application.Responses.Actors;
using Theatre.Errors;

namespace Theatre.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]/")]
public class ActorsController : Controller
{
    private readonly IActorsService _actorsService;

    public ActorsController(IActorsService actorsService)
    {
        _actorsService = actorsService;
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ActorFlat>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _actorsService.GetAll();

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return StatusCode(500);
    }
    
    [HttpGet("{actorId:guid}")]
    [ProducesResponseType(typeof(ActorFlat), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetActor([FromRoute] Guid actorId)
    {
        var result = await _actorsService.GetById(actorId);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        if (result.Error == DefinedErrors.Actors.ActorNotFound)
        {
            return NotFound(result.Error.Message);
        }

        return StatusCode(500);
    }

    [Authorize(Roles = IdentityRoles.Admin)]
    [HttpPost("")]
    [ProducesResponseType(typeof(CreateActorResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateActor([FromBody] CreateActorRequest request)
    {
        var result = await _actorsService.CreateActor(request);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        if (result.Error == DefinedErrors.Actors.ActorMustBeAdult
            || result.Error == DefinedErrors.Actors.ExperienceMustBeGreaterThanZero)
        {
            return BadRequest(result.Error.Message);
        }

        return StatusCode(500, result.Error.Message);
    }

    [HttpPut("")]
    [ProducesResponseType(typeof(ActorFlat), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdatePersonalInfo([FromBody] UpdatePersonalInfoRequest request)
    {
        if (User.IsInRole(IdentityRoles.Actor))
        {
            request.Id = Guid.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid).Value);
        }
        var result = await _actorsService.UpdateActor(request);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        if (result.Error == DefinedErrors.Actors.ActorMustBeAdult
            || result.Error == DefinedErrors.Actors.ExperienceMustBeGreaterThanZero)
        {
            return BadRequest(result.Error.Message);
        }

        return StatusCode(500, result.Error.Message);
    }

    [Authorize(Roles = IdentityRoles.Admin)]
    [HttpDelete("{actorId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteActor([FromRoute] Guid actorId)
    {
        var result = await _actorsService.DeleteActor(actorId);
        
        if (result.IsSuccess)
        {
            return Ok();
        }

        return StatusCode(500, result.Error.Message);
    }
}
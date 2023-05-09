using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Theatre.Application.Abstractions;
using Theatre.Application.Requests.Contracts;
using Theatre.Application.Responses.Contracts;
using Theatre.Errors;

namespace Theatre.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ContractController : Controller
{
    private readonly IContractService _contractService;

    public ContractController(IContractService contractService)
    {
        _contractService = contractService;
    }

    [HttpGet("")]
    [Authorize(Roles = IdentityRoles.Admin)]
    [ProducesResponseType(typeof(IEnumerable<ContractFullInfo>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllForAdmin()
    {
        var result = await _contractService.GetAll();

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return StatusCode(500, result.Error.Message);
    }
    
    [HttpGet("")]
    [Authorize(Roles = IdentityRoles.Actor)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string),StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllForActor()
    {
        Claim? idClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid);
        if (idClaim is null) return StatusCode(500);
        
        var result = await _contractService.GetByActor(Guid.Parse(idClaim.Value));
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

    [HttpGet("{contractId:guid}")]
    [Authorize(Roles = IdentityRoles.Actor)]
    [ProducesResponseType(typeof(ContractFullInfo), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(string),StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetContract([FromRoute] Guid contractId)
    {
        Claim? idClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid);
        if (idClaim is null) return StatusCode(500);
        var result = await _contractService.GetById(contractId);
        
        if (result.IsSuccess)
        {
            if (result.Value.Actor.Id.ToString() != idClaim.Value)
            {
                return Forbid("Actor can`t get not his contract");
            }
            
            return Ok(result);
        }

        if (result.Error == DefinedErrors.Contracts.ContractNotFound)
        {
            return NotFound(result.Error.Message);
        }

        return StatusCode(500, result.Error.Message);
    }
    
    [HttpGet("{contractId:guid}")]
    [Authorize(Roles = IdentityRoles.Admin)]
    [ProducesResponseType(typeof(ContractFullInfo), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string),StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetContractAdmin([FromRoute] Guid contractId)
    {
        var result = await _contractService.GetById(contractId);
        
        if (result.IsSuccess)
        {
            return Ok(result);
        }

        if (result.Error == DefinedErrors.Contracts.ContractNotFound)
        {
            return NotFound(result.Error.Message);
        }

        return StatusCode(500, result.Error.Message);
    }

    [HttpPost("")]
    [Authorize(Roles = IdentityRoles.Admin)]
    [ProducesResponseType(typeof(ContractFullInfo), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string),StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateContract([FromBody] CreateContractRequest request)
    {
        var result = await _contractService.Create(request);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        if (result.Error == DefinedErrors.Shows.ShowNotFound
            || result.Error == DefinedErrors.Actors.ActorNotFound
            || result.Error == DefinedErrors.Roles.RoleNotFound)
        {
            return NotFound(result.Error.Message);
        }

        if (result.Error == DefinedErrors.Contracts.BudgetOverdue
            || result.Error == DefinedErrors.Contracts.ContractAlreadyCreatedForRole)
        {
            return BadRequest(result.Error.Message);
        }

        return StatusCode(500, result.Error.Message);
    }

    [Authorize(Roles = IdentityRoles.Admin)]
    [HttpDelete("{contractId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteContract([FromRoute] Guid contractId)
    {
        var result = await _contractService.Delete(contractId);

        if (result.IsSuccess)
        {
            return Ok();
        }

        return StatusCode(500, result.Error.Message);
    }
}
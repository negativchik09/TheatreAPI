using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Theatre.Application.Requests.Contracts;
using Theatre.Application.Requests.Transactions;
using Theatre.Application.Services;
using Theatre.Errors;

namespace Theatre.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TransactionsController : Controller
{
    private readonly ContractService _contractService;

    public TransactionsController(ContractService contractService)
    {
        _contractService = contractService;
    }

    [HttpGet("")]
    [Authorize(Roles = IdentityRoles.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllForAdmin()
    {
        var result = await _contractService.GetAllTransactions();

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return StatusCode(500, result.Error.Message);
    }
    
    [HttpGet("")]
    [Authorize(Roles = IdentityRoles.Actor)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllForActor()
    {
        Claim? idClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid);
        if (idClaim is null) return StatusCode(500);

        var result = await _contractService.GetTransactionsByActor(Guid.Parse(idClaim.Value));

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        
        return StatusCode(500, result.Error.Message);
    }

    [Authorize(Roles = IdentityRoles.Admin)]
    [HttpPost("")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string),StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionRequest request)
    {
        var result = await _contractService.CreateTransaction(request);
        
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        if (result.Error == DefinedErrors.Contracts.ContractNotFound)
        {
            return NotFound(result.Error.Message);
        }
        
        return StatusCode(500, result.Error.Message);
    }
    
    [Authorize(Roles = IdentityRoles.Admin)]
    [HttpGet("{contractId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string),StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByContract([FromRoute] Guid contractId)
    {
        var result = await _contractService.GetTransactionsByContract(contractId);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        
        if (result.Error == DefinedErrors.Contracts.ContractNotFound)
        {
            return NotFound(result.Error.Message);
        }

        if (result.Error == DefinedErrors.Contracts.Overdraft)
        {
            return BadRequest(result.Error.Message);
        }
        
        return StatusCode(500, result.Error.Message);
    }
    
    [Authorize(Roles = IdentityRoles.Actor)]
    [HttpGet("{contractId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(string),StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByContractForActor([FromRoute] Guid contractId)
    {
        Claim? idClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid);
        if (idClaim is null) return StatusCode(500);
        var result = await _contractService.GetTransactionsByContract(contractId);
        
        if (result.IsSuccess)
        {
            if (result.Value.Any(x => x.ActorId.ToString() != idClaim.Value))
            {
                return Forbid("Actor can`t get not his transactions");
            }
            
            return Ok(result);
        }
        
        if (result.Error == DefinedErrors.Contracts.ContractNotFound)
        {
            return NotFound(result.Error.Message);
        }

        if (result.Error == DefinedErrors.Contracts.Overdraft)
        {
            return BadRequest(result.Error.Message);
        }
        
        return StatusCode(500, result.Error.Message);
    }
    
    [Authorize(Roles = IdentityRoles.Admin)]
    [HttpGet("by-actor/{actorId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    
    public async Task<IActionResult> GetByActor([FromRoute] Guid actorId)
    {
        var result = await _contractService.GetTransactionsByActor(actorId);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        
        return StatusCode(500, result.Error.Message);
    }
}
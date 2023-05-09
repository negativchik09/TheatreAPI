using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Theatre.Application.Requests.Contracts;
using Theatre.Application.Requests.Transactions;

namespace Theatre.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TransactionsController
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        
    }

    [HttpPost("")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionRequest request)
    {
        throw new NotImplementedException();
    }
    
    [HttpGet("{contractId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByContract([FromRoute] Guid contractId)
    {
        throw new NotImplementedException();
    }
    
    [HttpGet("by-actor/{actorId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByActor([FromRoute] Guid actorId)
    {
        throw new NotImplementedException();
    }
}
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Theatre.Application.Abstractions;
using Theatre.Application.Requests.Contracts;
using Theatre.Application.Requests.Transactions;
using Theatre.Application.Responses.Contracts;
using Theatre.Application.Responses.Transactions;
using Theatre.Domain;
using Theatre.Domain.Aggregates.Shows;
using Theatre.Errors;

namespace Theatre.Application.Services;

public class ContractService : IContractService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public ContractService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    private IQueryable<Contract> Contracts => _context.Contracts
        .Include(x => x.Transactions)
        .Include(x => x.Actor)
        .Include(x => x.Show)
        .Include(x => x.Role);

    public async Task<Result<IEnumerable<ContractFullInfo>>> GetAll(Func<Contract, bool>? filter = null)
    {
        var result = Contracts
            .AsNoTracking().AsEnumerable();

        if (filter is not null)
        {
            result = result.Where(filter);
        }

        return Result.Success(_mapper.Map<IEnumerable<ContractFullInfo>>(result));
    }

    public async Task<Result<ContractFullInfo>> GetById(Guid id)
    {
        var contract = await Contracts.FirstOrDefaultAsync(x => x.Id == id);

        if (contract is null)
        {
            return Result.Failure<ContractFullInfo>(DefinedErrors.Contracts.ContractNotFound);
        }

        return _mapper.Map<ContractFullInfo>(contract);
    }

    public async Task<Result<IEnumerable<ContractFullInfo>>> GetByActor(Guid actorId)
    {
        var isExist = await _context.Actors.AnyAsync(x => x.Id == actorId);
        if (!isExist)
        {
            return Result.Failure<IEnumerable<ContractFullInfo>>(DefinedErrors.Actors.ActorNotFound);
        }
        
        return await GetAll(contract => contract.ActorId == actorId);
    }

    public async Task<Result<ContractFullInfo>> Create(CreateContractRequest request)
    {
        var show = await _context.Shows
            .Include(x => x.Contracts)
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Id == request.ShowId);

        if (show is null)
        {
            return Result.Failure<ContractFullInfo>(DefinedErrors.Shows.ShowNotFound);
        }

        var actor = await _context.Actors.FirstOrDefaultAsync(x => x.Id == request.ActorId);
        if (actor is null)
        {
            return Result.Failure<ContractFullInfo>(DefinedErrors.Actors.ActorNotFound);
        }

        var contractResult = show.CreateContract(request.RoleId, request.ActorId, request.Sum);
        if (contractResult.IsSuccess)
        {
            await _context.AddAsync(contractResult.Value);
            await _context.SaveChangesAsync();
            return _mapper.Map<ContractFullInfo>(contractResult.Value);
        }
        
        return Result.Failure<ContractFullInfo>(contractResult.Error);
    }

    public async Task<Result> Delete(Guid id)
    {
        var contract = await _context.Contracts.FirstOrDefaultAsync(x => x.Id == id);
        if (contract is null)
        {
            return Result.Success();
        }

        _context.Contracts.Remove(contract);
        await _context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result<IEnumerable<TransactionFlat>>> GetAllTransactions(Func<Transaction, bool>? filter = null)
    {
        var result = _context.Transactions
            .AsNoTracking()
            .AsEnumerable();

        if (filter is not null)
        {
            result = result.Where(filter);
        }

        return Result.Success(_mapper.Map<IEnumerable<TransactionFlat>>(result));
    }

    public async Task<Result<TransactionFlat>> CreateTransaction(CreateTransactionRequest request)
    {
        var contract = await _context.Contracts
            .FirstOrDefaultAsync(x => x.Id == request.ContractId);

        if (contract is null)
        {
            return Result.Failure<TransactionFlat>(DefinedErrors.Contracts.ContractNotFound);
        }

        var contractResult = contract.CreateTransaction();
        if (contractResult.IsSuccess)
        {
            await _context.SaveChangesAsync();
            return _mapper.Map<TransactionFlat>(contractResult.Value);
        }
        
        return Result.Failure<TransactionFlat>(contractResult.Error);
    }

    public async Task<Result<IEnumerable<TransactionFlat>>> GetTransactionsByActor(Guid actorId)
    {
        if (!(await _context.Actors.AnyAsync(x => x.Id == actorId)))
        {
            return Result.Failure<IEnumerable<TransactionFlat>>(DefinedErrors.Actors.ActorNotFound);
        }
        
        var transactions = _context.Transactions
            .AsNoTracking()
            .Where(x => x.ActorId == actorId)
            .ToList();

        return Result.Success(_mapper.Map<IEnumerable<TransactionFlat>>(transactions));
    }

    public async Task<Result<IEnumerable<TransactionFlat>>> GetTransactionsByContract(Guid contractId)
    {
        var contract = await _context.Contracts
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == contractId);

        if (contract is null)
        {
            return Result.Failure<IEnumerable<TransactionFlat>>(DefinedErrors.Contracts.ContractNotFound);
        }
        
        return Result.Success(_mapper.Map<IEnumerable<TransactionFlat>>(contract.Transactions));
    }
}
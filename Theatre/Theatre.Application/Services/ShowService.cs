using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Theatre.Application.Abstractions;
using Theatre.Application.Requests.Contracts;
using Theatre.Application.Requests.Shows;
using Theatre.Application.Responses.Actors;
using Theatre.Application.Responses.Roles;
using Theatre.Application.Responses.Shows;
using Theatre.Domain;
using Theatre.Domain.Aggregates.Shows;
using Theatre.Errors;

namespace Theatre.Application.Services;

public class ShowService : IShowService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public ShowService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    private IQueryable<Show> Shows =>
        _context.Shows
            .Include(x => x.Contracts)
            .ThenInclude(x => x.Actor)
            .Include(x => x.Roles);

    public async Task<Result<IEnumerable<ShowTableView>>> GetAllFlat(Func<Show, bool>? filter = null)
    {
        var result = Shows
            .AsNoTracking().AsEnumerable();

        if (filter is not null)
        {
            result = result.Where(filter);
        }
        
        return Result.Success(_mapper.Map<IEnumerable<ShowTableView>>(result));
    }

    public async Task<Result<IEnumerable<ShowTableView>>> GetByActor(Guid actorId)
    {
        var shows = await GetAllFlat(
            show => show.Contracts.Any(contract => contract.ActorId == actorId));

        return Result.Success(_mapper.Map<IEnumerable<ShowTableView>>(shows));
    }

    private IEnumerable<RoleDescription> CreateRoleDescriptions(Show show)
    {
        return show.Roles
            .GroupJoin(
                show.Contracts, 
                role => role.Id, 
                contract => contract.RoleId, 
                (role, pair) => new { role, pair })
            .SelectMany(@t => @t.pair.DefaultIfEmpty(),
                (@t, subcontract) => 
                    new RoleDescription(
                        Id: @t.role.Id, 
                        Title: @t.role.Title,
                        Actor: _mapper.Map<ActorFlat>(subcontract?.Actor) ?? default, 
                        Contract: _mapper.Map<ContractFlat>(subcontract)));
    }

    public async Task<Result<ShowFullInfo>> GetById(Guid id)
    {
        var show = Shows.AsNoTracking().FirstOrDefault(x => x.Id == id);
        if (show is null)
        {
            return Result.Failure<ShowFullInfo>(DefinedErrors.Shows.ShowNotFound);
        }

        var roles = CreateRoleDescriptions(show);
        
        return new ShowFullInfo(show.Id,
            show.Title,
            show.TotalBudget.Amount,
            show.Contracts.Sum(x => x.YearCost.Amount),
            show.DateOfPremiere,
            roles);
    }

    public async Task<Result<ShowTableView>> CreateShow(CreateShowRequest request)
    {
        var showResult = Show.Create(
            request.Title,
            request.TotalBudget,
            request.DateOfPremiere);

        if (!showResult.IsSuccess)
        {
            return Result.Failure<ShowTableView>(showResult.Error);
        }

        var show = showResult.Value;

        await _context.AddAsync(show);

        await _context.SaveChangesAsync();

        return _mapper.Map<ShowTableView>(show);
    }
    
    public async Task<Result> DeleteShow(Guid showId)
    {
        var show = await _context.Shows.FirstOrDefaultAsync(x => x.Id == showId);

        if (show is null)
        {
            return Result.Success();
        }

        _context.Shows.Remove(show);
        
        await _context.SaveChangesAsync();
        
        return Result.Success();
    }
}
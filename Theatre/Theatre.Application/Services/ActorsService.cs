using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Theatre.Application.Abstractions;
using Theatre.Application.Requests.Actors;
using Theatre.Application.Responses.Actors;
using Theatre.Domain;
using Theatre.Domain.Aggregates.Actors;
using Theatre.Errors;

namespace Theatre.Application.Services;

public class ActorsService : IActorsService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IAccountService _accountService;

    public ActorsService(ApplicationDbContext dbContext, IMapper mapper, IAccountService accountService)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _accountService = accountService;
    }

    public async Task<Result<IEnumerable<ActorFlat>>> GetAll()
    {
        return Result.Success(_mapper.Map<IEnumerable<ActorFlat>>(_dbContext.Actors.AsNoTracking()));
    }

    public async Task<Result<ActorFlat>> GetById(Guid id)
    {
        var actor = await _dbContext.Actors
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        return actor == null 
            ? Result.Failure<ActorFlat>(DefinedErrors.Actors.ActorNotFound) 
            : _mapper.Map<ActorFlat>(actor);
    }

    public async Task<Result<CreateActorResponse>> CreateActor(CreateActorRequest request)
    {
        var userResult = await _accountService.CreateActor(request.Login, request.Email, request.Telephone);

        if (!userResult.IsSuccess)
        {
            return Result.Failure<CreateActorResponse>(userResult.Error);
        }

        var user = userResult.Value.User;

        var actorResult = Actor.Create(
            id: Guid.Parse(user.Id),
            firstName: request.FirstName,
            lastName: request.LastName,
            middleName: request.MiddleName,
            dateOfBirth: request.DateOfBirth,
            dignity: request.Dignity,
            experience: request.Experience,
            email: request.Email,
            telephone: request.Telephone,
            address: request.Address,
            number: request.PassportNumber,
            givenBy: request.PassportGivenBy,
            series: request.PassportSeries,
            taxesNumber: request.TaxesNumber);

        if (!actorResult.IsSuccess)
        {
            await _accountService.DeleteUser(Guid.Parse(user.Id));
            return Result.Failure<CreateActorResponse>(actorResult.Error);
        }

        _dbContext.Actors.Add(actorResult.Value);
        await _dbContext.SaveChangesAsync();

        return _mapper.Map<CreateActorResponse>(actorResult.Value) with
        {
            Login = request.Login,
            Password = userResult.Value.Password
        };
    }

    public async Task<Result<ActorFlat>> UpdateActor(UpdatePersonalInfoRequest request)
    {
        var actorResult = Actor.Create(
            id: request.Id.Value,
            firstName: request.FirstName,
            lastName: request.LastName,
            middleName: request.MiddleName,
            dateOfBirth: request.DateOfBirth,
            dignity: request.Dignity,
            experience: request.Experience,
            email: request.Email,
            telephone: request.Telephone,
            address: request.Address,
            number: request.PassportNumber,
            givenBy: request.PassportGivenBy,
            series: request.PassportSeries,
            taxesNumber: request.TaxesNumber);
        
        if (!actorResult.IsSuccess)
        {
            return Result.Failure<ActorFlat>(actorResult.Error);
        }

        _dbContext.Actors.Update(actorResult.Value);
        await _dbContext.SaveChangesAsync();

        return _mapper.Map<ActorFlat>(actorResult.Value);
    }

    public async Task<Result> DeleteActor(Guid actorId)
    {
        await _accountService.DeleteUser(actorId);
        
        var actor = await _dbContext.Actors.FirstOrDefaultAsync(x => x.Id == actorId);

        if (actor != null)
        {
            _dbContext.Remove(actor);
            await _dbContext.SaveChangesAsync();
        }
        
        return Result.Success();
    }
}
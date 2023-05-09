using Theatre.Application.Requests.Actors;
using Theatre.Application.Responses.Actors;
using Theatre.Errors;

namespace Theatre.Application.Abstractions;

public interface IActorsService
{
    Task<Result<IEnumerable<ActorFlat>>> GetAll();
    Task<Result<ActorFlat>> GetById(Guid id);
    Task<Result<CreateActorResponse>> CreateActor(CreateActorRequest request);
    Task<Result<ActorFlat>> UpdateActor(UpdatePersonalInfoRequest request);
    Task<Result> DeleteActor(Guid actorId);
}
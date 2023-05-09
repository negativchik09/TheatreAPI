using Theatre.Application.Requests.Shows;
using Theatre.Application.Responses.Shows;
using Theatre.Domain.Aggregates.Shows;
using Theatre.Errors;

namespace Theatre.Application.Abstractions;

public interface IShowService
{
    Task<Result<IEnumerable<ShowTableView>>> GetAllFlat(Func<Show, bool>? filter = null);
    Task<Result<IEnumerable<ShowTableView>>> GetByActor(Guid actorId);
    Task<Result<ShowFullInfo>> GetById(Guid id);
    Task<Result<ShowTableView>> CreateShow(CreateShowRequest request);
    Task<Result> DeleteShow(Guid showId);
}
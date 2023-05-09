using Theatre.Application.Responses.Actors;
using Theatre.Application.Responses.Roles;
using Theatre.Application.Responses.Shows;
using Theatre.Application.Responses.Transactions;

namespace Theatre.Application.Responses.Contracts;

public record ContractFullInfo(
    ActorFlat Actor,
    ShowFlat Show,
    RoleFlat RoleFlat,
    double YearCost,
    double AlreadyPayed,
    List<TransactionFlat> Transactions);
using Theatre.Application.Responses.Actors;
using Theatre.Application.Responses.Roles;
using Theatre.Application.Responses.Shows;
using Theatre.Application.Responses.Transactions;

namespace Theatre.Application.Responses.Contracts;

public record ContractFullInfo
{
    public ActorFlat Actor { get; set; }
    public ShowFlat Show { get; set; }
    public RoleFlat RoleFlat { get; set; }
    public double YearCost { get; set; }
    public double AlreadyPayed { get; set; }
    public List<TransactionFlat> Transactions { get; set; }
}
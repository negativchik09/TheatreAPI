using Theatre.Domain.Abstractions;
using Theatre.Domain.Aggregates.Actors;
using Theatre.Domain.Primitives;
using Theatre.Errors;

namespace Theatre.Domain.Aggregates.Shows;

public class Contract : Entity
{
    private List<Transaction> _transactions = new();
    public Guid ActorId { get; private set; }
    public Actor Actor { get; private set; }
    public Guid ShowId { get; private set; }
    public Show Show { get; private set; }
    public Guid RoleId { get; private set; }
    public Role Role { get; private set; }
    public Money YearCost { get; private set; }

    public IReadOnlyList<Transaction> Transactions => _transactions;

    public Result<Transaction> CreateTransaction()
    {
        if (Transactions.Count == 12)
        {
            return Result.Failure<Transaction>(DefinedErrors.Contracts.Overdraft);
        }
        
        var transaction = new Transaction(
            new Guid(),
            Id,
            ActorId,
            Money.Create(YearCost.Amount / 12).Value, // One month
            DateTime.UtcNow);
        
        _transactions.Add(transaction);
        return transaction;
    }

    internal Contract(Guid id, Guid actorId, Guid showId, Guid roleId, Money yearCost)
    {
        Id = id;
        ActorId = actorId;
        ShowId = showId;
        RoleId = roleId;
        YearCost = yearCost;
    }
}
using Theatre.Domain.Abstractions;
using Theatre.Domain.Primitives;

namespace Theatre.Domain.Aggregates.Shows;

public class Transaction : Entity
{
    public Guid ContractId { get; private set; }
    public Guid ActorId { get; private set; }

    public Money Sum { get; private set; }

    public DateTime Date { get; private set; }

    internal Transaction(Guid id, Guid contractId, Guid actorId, Money sum, DateTime date)
    {
        Id = id;
        ContractId = contractId;
        ActorId = actorId;
        Sum = sum;
        Date = date;
    }

    private Transaction() { }
}
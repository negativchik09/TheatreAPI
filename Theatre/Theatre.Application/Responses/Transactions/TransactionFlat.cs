namespace Theatre.Application.Responses.Transactions;

public record TransactionFlat(
    Guid Id,
    Guid ContractId,
    Guid ActorId,
    double Sum, 
    DateTime Date);
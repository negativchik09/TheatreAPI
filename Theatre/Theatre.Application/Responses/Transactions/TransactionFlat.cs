namespace Theatre.Application.Responses.Transactions;

public record TransactionFlat(
    Guid Id,
    Guid ContractId,
    Guid ActorId,
    string ActorFullName,
    double Sum, 
    DateTime Date);
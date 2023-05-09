namespace Theatre.Application.Requests.Transactions;

public record CreateTransactionRequest(
    Guid ContractId);
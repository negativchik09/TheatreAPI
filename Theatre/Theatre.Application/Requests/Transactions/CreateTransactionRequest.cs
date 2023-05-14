namespace Theatre.Application.Requests.Transactions;

public record CreateTransactionRequest
{
    public Guid ContractId { get; set; }
}
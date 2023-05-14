namespace Theatre.Application.Responses.Transactions;

public record TransactionFlat
{
    public Guid Id { get; set; }
    public Guid ContractId { get; set; }
    public Guid ActorId { get; set; }
    public double Sum { get; set; }
    public DateTime Date { get; set; }
}
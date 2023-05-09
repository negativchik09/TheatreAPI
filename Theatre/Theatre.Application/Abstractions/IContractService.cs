using Theatre.Application.Requests.Contracts;
using Theatre.Application.Requests.Transactions;
using Theatre.Application.Responses.Contracts;
using Theatre.Application.Responses.Transactions;
using Theatre.Domain.Aggregates.Shows;
using Theatre.Errors;

namespace Theatre.Application.Abstractions;

public interface IContractService
{
    Task<Result<IEnumerable<ContractFullInfo>>> GetAll(Func<Contract, bool>? filter = null);
    Task<Result<ContractFullInfo>> GetById(Guid id);
    Task<Result<IEnumerable<ContractFullInfo>>> GetByActor(Guid actorId);
    Task<Result<ContractFullInfo>> Create(CreateContractRequest request);
    Task<Result> Delete(Guid id);
    Task<Result<IEnumerable<TransactionFlat>>> GetAllTransactions(Func<Transaction, bool>? filter = null);
    Task<Result<IEnumerable<TransactionFlat>>> GetTransactionsByActor(Guid actorId);
    Task<Result<IEnumerable<TransactionFlat>>> GetTransactionsByContract(Guid contractId);
    Task<Result<TransactionFlat>> CreateTransaction(CreateTransactionRequest request);
}
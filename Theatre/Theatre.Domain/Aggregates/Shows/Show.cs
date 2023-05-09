using Theatre.Domain.Abstractions;
using Theatre.Domain.Primitives;
using Theatre.Errors;

namespace Theatre.Domain.Aggregates.Shows;

public class Show : Entity
{
    private readonly List<Role> _roles = new();
    private readonly List<Contract> _contracts = new();
    public string Title { get; private set; }
    public Money TotalBudget { get; private set; }
    public DateTime DateOfPremiere { get; private set; }
    public IReadOnlyList<Role> Roles => _roles;
    public IReadOnlyList<Contract> Contracts => _contracts;

    private Show(Guid id, string title, Money totalBudget, DateTime dateOfPremiere)
    {
        Id = id;
        Title = title;
        TotalBudget = totalBudget;
        DateOfPremiere = dateOfPremiere;
    }

    public static Result<Show> Create(string title, double totalBudget, DateTime dateOfPremiere)
    {
        var budgetResult = Money.Create(totalBudget);
        if (!budgetResult.IsSuccess)
        {
            return Result.Failure<Show>(budgetResult.Error);
        }

        return new Show(
            new Guid(),
            title,
            budgetResult.Value,
            dateOfPremiere);
    }

    public Result<Role> AddRole(string roleTitle)
    {
        if (Roles.Any(x => x.Title == roleTitle))
        {
            return Result.Failure<Role>(DefinedErrors.Roles.RoleAlreadyCreatedForShow);
        }
        
        var role = new Role(new Guid(), Id, roleTitle);
        _roles.Add(role);

        return role;
    }
    
    public Result<Contract> CreateContract(Guid roleId, Guid actorId, double yearCost)
    {
        var moneyResult = Money.Create(yearCost);
        if (!moneyResult.IsSuccess)
        {
            return Result.Failure<Contract>(moneyResult.Error);
        }

        var alreadySpent = Contracts.Sum(x => x.YearCost.Amount);
        if (yearCost + alreadySpent > TotalBudget.Amount)
        {
            return Result.Failure<Contract>(Errors.DefinedErrors.Contracts.BudgetOverdue);
        }

        if (Roles.All(x => x.Id != roleId))
        {
            return Result.Failure<Contract>(Errors.DefinedErrors.Roles.RoleNotFound);
        }
        
        if (Contracts.Any(x => x.RoleId == roleId))
        {
            return Result.Failure<Contract>(Errors.DefinedErrors.Contracts.ContractAlreadyCreatedForRole);
        }
        
        var contract = new Contract(
            id: new Guid(),
            actorId: actorId,
            showId: Id,
            roleId: roleId,
            yearCost: moneyResult.Value);
        
        _contracts.Add(contract);

        return contract;
    }
}
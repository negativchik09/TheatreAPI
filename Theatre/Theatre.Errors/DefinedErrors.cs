namespace Theatre.Errors;

public static class DefinedErrors
{
    public static class Money
    {
        public static readonly Error MoneyMustBeGreaterThanZero = new("Money must be grater than zero");
    }
    
    public static class Actors
    {
        public static readonly Error ActorNotFound = new("Actor not found");
        public static readonly Error ActorMustBeAdult = new("Actor must be adult");
        public static readonly Error ExperienceMustBeGreaterThanZero = new("Experience must be greater than zero");
        public static readonly Error NameIssue = new("Invalid name");
    }
    
    public static class Contracts
    {
        public static readonly Error ContractNotFound = new("Contract not found");
        public static readonly Error BudgetOverdue = new("It`s not enough money to create contract");
        public static readonly Error RoleNotFound = new("Role not found");
        public static readonly Error ContractAlreadyCreatedForRole =
            new("For provided role contract have already been created");
        public static readonly Error Overdraft = new("All money have been payed on this contract");
    }
    public static class Accounts
    {
        public static readonly Error UserNotFound = new("User not found");
        public static readonly Error Identity = new("Identity error");
    }

    public static class Shows
    {
        public static readonly Error ShowNotFound = new("Show not found");
    }
}
using Theatre.Errors;

namespace Theatre.Domain.Primitives;

public record Money
{
    public double Amount { get; init; }

    private Money(double value)
    {
        Amount = value;
    }

    public static Result<Money> Create(double value)
    {
        return value > 0 
            ? new Money(value) 
            : Result.Failure<Money>(Errors.DefinedErrors.Money.MoneyMustBeGreaterThanZero);
    }
}
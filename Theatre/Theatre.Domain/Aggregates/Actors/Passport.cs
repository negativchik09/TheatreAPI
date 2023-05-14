namespace Theatre.Domain.Aggregates.Actors;

public record Passport
{
    public string Number { get; private set; }
    public string GivenBy { get; private set; }
    public string? Series { get; private set; }

    private Passport(string number, string givenBy, string? series)
    {
        Series = series;
        Number = number;
        GivenBy = givenBy;
    }

    public static Passport Create(string number, string givenBy, string? series)
    {
        return new Passport(number, givenBy, series);
    }
}
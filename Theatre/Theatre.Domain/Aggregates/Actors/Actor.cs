using System.Text.RegularExpressions;
using Theatre.Domain.Abstractions;
using Theatre.Errors;

namespace Theatre.Domain.Aggregates.Actors;

public class Actor : Entity
{
    public FullName Name { get; private set; }

    public DateTime DateOfBirth { get; private set; }

    public string Dignity { get; private set; }

    public double Experience { get; private set; }
    public string Email { get; private set; }
    public string Telephone { get; private set; }
    public string Address { get; private set; }
    public Passport Passport { get; private set; }
    public string TaxesNumber { get; private set; }

    private Actor() { }

    private Actor(
        Guid id, 
        FullName name, 
        DateTime dateOfBirth, 
        string dignity, 
        double experience, 
        string email, 
        string telephone, 
        string address,
        Passport passport,
        string taxesNumber)
    {
        Id = id;
        Name = name;
        DateOfBirth = dateOfBirth;
        Dignity = dignity;
        Experience = experience;
        Email = email;
        Telephone = telephone;
        Address = address;
        Passport = passport;
        TaxesNumber = taxesNumber;
    }

    public static Result<Actor> Create(
        Guid id,
        string firstName, 
        string lastName, 
        string middleName, 
        DateTime dateOfBirth,
        string dignity, 
        double experience,
        string email, 
        string telephone, 
        string address,
        string number,
        string givenBy,
        string? series,
        string taxesNumber)
    {
        if (dateOfBirth.AddYears(18) > DateTime.UtcNow)
        {
            return Result.Failure<Actor>(DefinedErrors.Actors.ActorMustBeAdult);
        }

        if (experience < 0)
        {
            return Result.Failure<Actor>(DefinedErrors.Actors.ExperienceMustBeGreaterThanZero);
        }
        
        // Regular expression pattern for Cyrillic letters
        string pattern = @"\p{IsCyrillic}";
        
        if (!Regex.IsMatch(firstName, pattern))
        {
            return Result.Failure<Actor>(DefinedErrors.Actors.NameIssue);
        }
        if (!Regex.IsMatch(lastName, pattern))
        {
            return Result.Failure<Actor>(DefinedErrors.Actors.NameIssue);
        }
        if (!Regex.IsMatch(middleName, pattern))
        {
            return Result.Failure<Actor>(DefinedErrors.Actors.NameIssue);
        }

        var passport = Passport.Create(number, givenBy, series);
        // TODO: taxesNumber validation
        return new Actor(
            id: id,
            name: new FullName(firstName, lastName, middleName),
            dateOfBirth: dateOfBirth,
            dignity: dignity,
            experience: experience,
            email: email,
            telephone: telephone,
            address: address,
            passport: passport,
            taxesNumber: taxesNumber
            );
    }
}
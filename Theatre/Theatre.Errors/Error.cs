namespace Theatre.Errors;

public class Error : IEquatable<Error>
{
    public static readonly Error None = new(string.Empty);

    public Error(string message)
    {
        Message = message;
    }

    public string Message { get; }

    public static implicit operator string(Error error) => error.Message;


    public bool Equals(Error? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Message == other.Message;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Error)obj);
    }

    public override int GetHashCode()
    {
        return Message.GetHashCode() * 397;
    }

    public static bool operator ==(Error? a, Error? b)
    {
        if (a is null && b is null)
        {
            return true;
        }

        if (a is null || b is null)
        {
            return false;
        }

        return a.Equals(b);
    }

    public static bool operator !=(Error? a, Error? b)
    {
        return !(a == b);
    }
}
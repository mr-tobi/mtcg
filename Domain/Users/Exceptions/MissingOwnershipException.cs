namespace Domain.Users.Exceptions;

public class MissingOwnershipException : Exception
{
    public MissingOwnershipException(string entityType) : base($"{entityType} does not belong to user")
    {
    }
}

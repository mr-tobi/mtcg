namespace Domain.Shop.Exceptions;

public class NotSellableException : Exception
{
    public NotSellableException(string? entityType) : base($"{entityType} could not be sold")
    {
    }
}

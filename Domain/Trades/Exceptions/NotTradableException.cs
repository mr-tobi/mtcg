namespace Domain.Trades.Exceptions;

public class NotTradableException : Exception
{
    public NotTradableException(string message) : base($"Card is not tradable: {message}")
    {
    }
}

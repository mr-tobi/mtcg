namespace Domain.Users.Exceptions;

public class NotEnoughMoneyException : Exception
{
    public NotEnoughMoneyException() : base("User does not have enough money")
    {
    }
}

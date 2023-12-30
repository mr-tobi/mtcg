namespace Application.User.Exceptions;

public class DeckSizeException : Exception
{
    public DeckSizeException() : base("The provided deck did not include the required amount of cards")
    {
    }
}

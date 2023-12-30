namespace Domain.Cards;

public class UUID
{
    public string Id { get; }

    public UUID()
    {
        Id = Guid.NewGuid().ToString();
    }

    public UUID(string id)
    {
        Id = id;
    }

    public static string NewUUID()
    {
        return Guid.NewGuid().ToString();
    }

    public override bool Equals(object? obj)
    {
        return obj?.GetType() == typeof(UUID) && ((UUID)obj).Id == Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}

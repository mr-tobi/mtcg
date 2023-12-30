namespace Application.Package;

public class CardData
{
    public string? Id { get; }
    public string Name { get; }
    public float Damage { get; }

    public CardData(string? id, string name, float damage)
    {
        Id = id;
        Name = name;
        Damage = damage;
    }
}
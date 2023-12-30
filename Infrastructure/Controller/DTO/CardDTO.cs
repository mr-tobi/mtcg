namespace Infrastructure.Controller.DTO;

public class CardDTO
{
    public string? Id { get; }
    public string Name { get; }
    public float Damage { get; }

    public CardDTO(string? id, string name, float damage)
    {
        Id = id;
        Name = name;
        Damage = damage;
    }
}
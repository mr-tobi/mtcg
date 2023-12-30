namespace Infrastructure.Controller.DTO;

public class TradeOfferDTO
{
    public string Id { get; }
    public string CardToTrade { get; }
    public string Type { get; }
    public int MinimumDamage { get; }

    public TradeOfferDTO(string id, string cardToTrade, string type, int minimumDamage)
    {
        Id = id;
        CardToTrade = cardToTrade;
        Type = type;
        MinimumDamage = minimumDamage;
    }
}

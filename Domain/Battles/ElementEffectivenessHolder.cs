using Domain.Cards;

namespace Domain.Battles;

public class ElementEffectivenessHolder
{
    private readonly Dictionary<CardElement, Dictionary<CardElement, float>> _elementEffectiveness = new()
    {
        {
            CardElement.Water, new Dictionary<CardElement, float>
            {
                { CardElement.Water, 1 },
                { CardElement.Fire, 2 },
                { CardElement.Normal, 0.5f },
            }
        },
        {
            CardElement.Fire, new Dictionary<CardElement, float>
            {
                { CardElement.Water, 0.5f },
                { CardElement.Fire, 1 },
                { CardElement.Normal, 2 },
            }
        },
        {
            CardElement.Normal, new Dictionary<CardElement, float>
            {
                { CardElement.Water, 2 },
                { CardElement.Fire, 0.5f },
                { CardElement.Normal, 1 },
            }
        }
    };

    public float GetDamageMultiplier(CardElement attackingCardElement, CardElement defendingCardElement)
    {
        try
        {
            return _elementEffectiveness[attackingCardElement][defendingCardElement];
        }
        catch (KeyNotFoundException e)
        {
            throw new InvalidOperationException("Element is not configured");
        }
    }
}
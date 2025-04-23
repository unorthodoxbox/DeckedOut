using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Card/CardDropTable")]
public class CardDropTable : ScriptableObject
{
    [System.Serializable]
    public class CardDrop
    {
        public Card card;
        public float weight;
    }

    public List<CardDrop> drops;

    public Card GetRandomCard()
    {
        float totalWeight = 0f;
        foreach (var drop in drops)
            totalWeight += drop.weight;

        float roll = Random.value * totalWeight;
        float cumulative = 0f;

        foreach (var drop in drops)
        {
            cumulative += drop.weight;
            if (roll <= cumulative)
                return drop.card;
        }

        return null; // fallback
    }
}

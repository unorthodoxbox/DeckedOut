using UnityEngine;

public abstract class Card : ScriptableObject
{
    public string cardName;
    public string description;
    public Sprite icon;

    // Abstract method to apply the effect of the card
    public abstract void ApplyEffect(EntityStats entity);
}


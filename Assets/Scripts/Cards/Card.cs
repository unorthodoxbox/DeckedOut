using UnityEngine;

public abstract class Card : ScriptableObject
{
    public string cardName;
    public string description;

    // Method to apply card effects
    public abstract void ApplyEffect(PlayerStats player);
}   

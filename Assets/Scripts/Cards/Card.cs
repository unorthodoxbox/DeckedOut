using UnityEngine;

public abstract class Card : ScriptableObject
{
    public string cardName;
    public string description;
    public Sprite icon;

    public GameObject cardPrefab; // The 3D object (card model) associated with the card

    public abstract void ApplyEffect(EntityStats entity);


    // Additional logic to spawn the 3D model in the world
    public GameObject SpawnCard(Vector3 position)
    {
        if (cardPrefab != null)
        {
            GameObject cardObject = Instantiate(cardPrefab, position, Quaternion.identity);
            // You can add any additional initialization logic here (e.g., adding colliders, etc.)
            return cardObject;
        }
        return null;
    }
}

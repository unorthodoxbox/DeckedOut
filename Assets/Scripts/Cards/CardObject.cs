using UnityEngine;

public class CardObject : MonoBehaviour
{
    public Card cardData;  // The ScriptableObject data for the card

    private void Start()
    {
        if (cardData != null)
        {
            Debug.Log($"This object contains the card: {cardData.cardName}");
        }
    }

    // This function will be called when the card collides with another collider (trigger)
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that collided has the "Player" tag
        if (other.CompareTag("Player"))
        {
            // If it has the "Player" tag, get the EntityStats component
            EntityStats entityStats = other.GetComponent<EntityStats>();

            if (entityStats != null && cardData != null)
            {
                // Apply the effect of the card to the player's stats
                cardData.ApplyEffect(entityStats);
                Debug.Log($"Applied {cardData.cardName} to player!");

                // Optional: Destroy the card object after applying the effect
                Destroy(gameObject);
            }
        }
    }
}



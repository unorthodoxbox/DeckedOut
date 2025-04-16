using UnityEngine;

public class CardObject : MonoBehaviour
{
    public Card cardData;  // The ScriptableObject data for the card

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            cardData.ApplyEffect(other.gameObject.GetComponent<EntityStats>());
            Destroy(this.gameObject);
        }
    }
    
}



using UnityEngine;

public class CardObject : MonoBehaviour
{
    public Card cardData;  // The ScriptableObject data for the card

    public GameObject cardPrefab;

    private GameObject cardInstance;

    private void Start()
    {
        if (cardPrefab != null)
        {
            cardInstance = Instantiate(cardPrefab, this.transform.position, this.transform.rotation, this.transform);
            cardInstance.AddComponent<CardCollisionHandler>();
        } else
        {
            Debug.Log("cardPrefab is null");
        }
    }
    public void handleCollision(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            cardData.ApplyEffect(other.GetComponent<EntityStats>());
            Destroy(cardInstance);
            Destroy(this);
        }
    }

}



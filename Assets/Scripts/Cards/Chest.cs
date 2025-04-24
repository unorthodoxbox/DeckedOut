using UnityEngine;
using TMPro;

public class Chest : MonoBehaviour
{
    public int cost = 0;
    public CardDropTable dropTable;
    private bool isOpened = false;

    [Header("UI")]
    public GameObject uiCanvas;
    public TextMeshProUGUI costText;
    public float showDistance = 3f;

    [Header("Card Spawn")]
    public GameObject cardObjectPrefab; // Assign your CardObject prefab in Inspector
    public Transform cardSpawnPoint;    // Optional: set a Transform above chest to spawn card

    private Transform playerCamera;
    private EntityStats playerStatsNearby;

    private void Start()
    {
        cost = Random.Range(25, 51); // inclusive

        playerCamera = Camera.main?.transform;

        if (uiCanvas != null)
        {
            uiCanvas.SetActive(false);
            costText.text = "$" + cost;
        }
    }   

    private void Update()
    {
        // UI Display Logic
        if (uiCanvas != null && playerCamera != null && !isOpened)
        {
            float dist = Vector3.Distance(playerCamera.position, transform.position);
            Vector3 dirToChest = (transform.position - playerCamera.position).normalized;
            float dot = Vector3.Dot(playerCamera.forward, dirToChest);

            if (dist < showDistance && dot > 0.7f)
            {
                uiCanvas.SetActive(true);
                uiCanvas.transform.rotation = Quaternion.LookRotation(uiCanvas.transform.position - playerCamera.position);
            }
            else
            {
                uiCanvas.SetActive(false);
            }
        }

        // Interaction Logic
        if (playerStatsNearby != null && Input.GetKeyDown(KeyCode.E))
        {
            TryOpen(playerStatsNearby);
        }
    }

    public void TryOpen(EntityStats playerStats)
    {
        if (isOpened || playerStats.currency < cost)
            return;

        playerStats.currency -= cost;
        isOpened = true;

        // Get a random card from the loot table
        Card randomCard = dropTable.GetRandomCard();

        // Instantiate the card object in the world
        Vector3 spawnPos = cardSpawnPoint != null ? cardSpawnPoint.position : transform.position + Vector3.up * 1.5f;
        GameObject cardGO = Instantiate(cardObjectPrefab, spawnPos, Quaternion.identity);

        // Assign the card data to the CardObject component
        CardObject cardObj = cardGO.GetComponent<CardObject>();
        if (cardObj != null)
        {
            cardObj.cardData = randomCard;
        }

        Debug.Log($"Spawned card: {randomCard.cardName}");

        if (uiCanvas) uiCanvas.SetActive(false);

        // Optional: play animation/sound here
        Destroy(gameObject);    
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerStatsNearby = other.GetComponent<EntityStats>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerStatsNearby = null;
            if (uiCanvas) uiCanvas.SetActive(false);
        }
    }
}

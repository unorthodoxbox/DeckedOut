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

        Card randomCard = dropTable.GetRandomCard();
        randomCard.ApplyEffect(playerStats);

        Debug.Log($"Player got: {randomCard.cardName}");

        if (uiCanvas) uiCanvas.SetActive(false);

        // Optional: play animation/sound here
        Destroy(gameObject, 2f);
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

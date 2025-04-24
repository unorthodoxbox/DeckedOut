using UnityEngine;
using TMPro;

public class CardObject : MonoBehaviour
{
    public Card cardData;  // ScriptableObject for the card
    public GameObject uiCanvas; // World-space canvas GameObject
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descText;

    public float showDistance = 3f;
    private Transform playerCamera;
    private GameObject player;

    private Vector3 startPos;
    public float floatAmplitude = 0.25f;
    public float floatFrequency = 1f;
    public float spinSpeed = 30f;

    private bool canCollect = false;

    private void Start()
    {
        startPos = transform.position;
        playerCamera = Camera.main.transform;
        player = GameObject.FindGameObjectWithTag("Player");

        if (uiCanvas != null && cardData != null)
        {
            uiCanvas.SetActive(false);
            nameText.text = cardData.cardName;
            descText.text = cardData.description;
        }
    }

    private void Update()
    {
        // Floating motion
        float newY = startPos.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // Spinning
        transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.World);

        // UI and interaction logic
        if (playerCamera != null && uiCanvas != null)
        {
            float dist = Vector3.Distance(playerCamera.position, transform.position);
            Vector3 dirToCard = (transform.position - playerCamera.position).normalized;
            float dot = Vector3.Dot(playerCamera.forward, dirToCard);

            if (dist < showDistance && dot > 0.7f)
            {
                uiCanvas.SetActive(true);
                uiCanvas.transform.rotation = Quaternion.LookRotation(uiCanvas.transform.position - playerCamera.position);
                canCollect = true;

                if (Input.GetKeyDown(KeyCode.E))
                {
                    CollectCard();
                }
            }
            else
            {
                uiCanvas.SetActive(false);
                canCollect = false;
            }
        }
    }

    private void CollectCard()
    {
        if (cardData != null && player != null)
        {
            cardData.ApplyEffect(player.GetComponent<EntityStats>());
            GameObject.Find("Audio Manager").GetComponent<AudioManager>().PlayOneShot("Card Collected");
            Destroy(this.gameObject);
        }
    }
}

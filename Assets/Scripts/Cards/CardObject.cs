using UnityEngine;

public class CardObject : MonoBehaviour
{
    public Card cardData;  // The ScriptableObject data for the card

    private Vector3 startPos;
    public float floatAmplitude = 0.25f;   // How high it floats
    public float floatFrequency = 1f;      // How fast it floats
    public float spinSpeed = 30f;          // How fast it spins

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        // Floating motion
        float newY = startPos.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // Spinning
        transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            cardData.ApplyEffect(other.gameObject.GetComponent<EntityStats>());
            Destroy(this.gameObject);
        }
    }
}

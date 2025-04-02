using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject player;

    private float damage = 0f;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }


    //Better solution is an invisible box around map, but that can wait till map actually exists
    void Update() {
        if (gameObject.transform.position.x - player.transform.position.x > 50 || gameObject.transform.position.y - player.transform.position.y > 50 || gameObject.transform.position.z - player.transform.position.z > 50) {
            Destroy(gameObject);
        }
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy" && gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<EntityStats>().takeDamage(damage);
        }
        Destroy(gameObject);
    }

    public void InitializeVariables(string tag, float damage, float speed, Vector3 direction)
    {
        gameObject.tag = tag;
        this.damage = damage;

        // Move the bullet using Rigidbody physics
        rb.linearVelocity = direction * speed;
    }
}

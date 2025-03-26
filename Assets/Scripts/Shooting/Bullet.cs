using UnityEngine;

public class Bullet : MonoBehaviour
{

    private float damage = 0f;
    private float speed = 20f;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
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
        this.speed = speed;

        // Move the bullet using Rigidbody physics
        rb.linearVelocity = direction * speed;
    }
}

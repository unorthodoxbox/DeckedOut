using UnityEngine;

public class Bullet : MonoBehaviour
{

    private float damage = 0f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, 10f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            other.gameObject.GetComponent<EntityStats>().takeDamage(damage);
        }
        Destroy(gameObject);
    }

    public void InitializeVariables(float damage)
    {
        this.damage = damage;
    }
}

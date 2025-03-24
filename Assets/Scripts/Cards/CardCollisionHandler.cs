using UnityEngine;

public class CardCollisionHandler : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        transform.parent.GetComponent<CardObject>().handleCollision(other);
    }
}

using UnityEngine;

public class Melee : MonoBehaviour
{
    float damage = 10f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            other.gameObject.GetComponent<EntityStats>().takeDamage(damage);
            AudioManager.weaponPlayer.GetComponent<SoundPlayer>().Play("Melee Swing");
        }
    }
}

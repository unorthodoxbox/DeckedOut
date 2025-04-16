using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AmmoUI : MonoBehaviour
{
    public GameObject player;
    private EntityStats playerStats;


    public GameObject ammoUI;

    private float ammoInGun;
    private float totalAmmo;


    void Awake()
    {
        //player = GameObject.FindGameObjectWithTag("Player");
        playerStats = player.GetComponent<EntityStats>();
        ammoInGun = playerStats.ammoInGun;
        totalAmmo = playerStats.totalAmmo;
        ammoUI.GetComponent<TextMeshProUGUI>().text = ammoInGun + "\\" + totalAmmo;
    }

    // Update is called once per frame
    void Update()
    {
        if (player.GetComponent<ThirdPersonController>().currentWeaponIndex == 1) {
            ammoUI.GetComponent<TextMeshProUGUI>().text = "∞/∞";
        } else {
            ammoUI.GetComponent<TextMeshProUGUI>().text = ammoInGun + "\\" + totalAmmo;
            if (ammoInGun != playerStats.ammoInGun) {
                ammoInGun = playerStats.ammoInGun;
            }           
            if (totalAmmo != playerStats.totalAmmo) {
                totalAmmo = playerStats.totalAmmo;
            }
        }
    }
}

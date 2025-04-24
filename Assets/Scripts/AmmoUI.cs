using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AmmoUI : MonoBehaviour
{
    public GameObject player;
    private EntityStats playerStats;


    public TextMeshProUGUI ammoUI;
    public TextMeshProUGUI currencyUI;
    public TextMeshProUGUI currentWaveText;
    public Slider waveProgressSlider;

    private float ammoInGun;
    private float totalAmmo;
    private int totalCurrency;

    //public int totalEnemies = 6;
    //public int killedEnemies = 0;


    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerStats = player.GetComponent<EntityStats>();
        ammoInGun = playerStats.ammoInGun;
        totalAmmo = playerStats.totalAmmo;
        totalCurrency = playerStats.currency;
        //ammoUI.GetComponent<TextMeshProUGUI>().text = ammoInGun + "\\" + totalAmmo;

        //waveProgressSlider.maxValue = totalEnemies;
        waveProgressSlider.maxValue = GameManager.Instance.waveSize;
        //waveProgressSlider.value = 20;
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

        if (playerStats.currency != totalCurrency)
        {
            totalCurrency = playerStats.currency;
            currencyUI.GetComponent<TextMeshProUGUI>().text = "$" + totalCurrency;
        }
        currentWaveText.text = "Wave " + GameManager.Instance.currentWave;
        waveProgressSlider.maxValue = GameManager.Instance.waveSize;
        waveProgressSlider.value = waveProgressSlider.maxValue - GameManager.Instance.numEnemies;
    }
}

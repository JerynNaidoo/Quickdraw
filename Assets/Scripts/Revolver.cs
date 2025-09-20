using UnityEngine;
using TMPro;

public class Revolver : MonoBehaviour
{
    [Header("Ammo Settings")]
    public int maxAmmo = 6;          // revolver cylinder size
    public int currentAmmo;          // ammo in cylinder
    public int reserveAmmo = 30;     // total spare bullets (not including cylinder)

    [Header("References")]
    public TextMeshProUGUI ammoText;

    void Start()
    {
        currentAmmo = maxAmmo; // start full
        UpdateAmmoUI();
    }

    void Update()
    {
        //if (Input.GetButtonDown("Fire1")) // left click
        //{
        //    Shoot();
        //}

        if (Input.GetKeyDown(KeyCode.R)) // press R to reload
        {
            Reload();
        }
    }

    public bool CanShoot()
    {
        return currentAmmo > 0;
    }

    public void ConsumeAmmo()
    {
        if (currentAmmo > 0)
        {
            currentAmmo--;
            Debug.Log("Bang! Chamber: " + currentAmmo + " | Reserve: " + reserveAmmo);
            UpdateAmmoUI();
        }
        else
        {
            Debug.Log("Click! Out of ammo in cylinder!");
        }
    }

    void Reload()
    {
        if (reserveAmmo <= 0 || currentAmmo == maxAmmo)
        {
            Debug.Log("No need or no ammo left to reload!");
            return;
        }

        int needed = maxAmmo - currentAmmo;          // bullets required to fill cylinder
        int bulletsToLoad = Mathf.Min(needed, reserveAmmo); // load only what's available

        currentAmmo += bulletsToLoad;
        reserveAmmo -= bulletsToLoad;

        Debug.Log("Reloaded! Chamber: " + currentAmmo + " / " + maxAmmo + " | Reserve: " + reserveAmmo);
        UpdateAmmoUI();
    }

    void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = currentAmmo + " / " + reserveAmmo;
        }
    }

    public void AddReserveAmmo(int amount)
    {
        reserveAmmo += amount;
        Debug.Log("Picked up ammo crate! Reserve now: " + reserveAmmo);
        UpdateAmmoUI();
    }


}

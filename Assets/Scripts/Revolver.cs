using UnityEngine;
using TMPro;

public class Revolver : MonoBehaviour
{
    [Header("Ammo Settings")]
    public int maxAmmo = 6;          // Revolver cylinder size
    public int currentAmmo;          // Ammo in cylinder
    public int reserveAmmo = 30;     // Total spare bullets (not including cylinder)

    [SerializeField]
    private GameObject player;       // Reference to player (for animator)
    private Animator pistolAnimator;

    [Header("References")]
    public TextMeshProUGUI ammoText;

    void Start()
    {
        currentAmmo = maxAmmo; // Start with a full cylinder
        UpdateAmmoUI();

        pistolAnimator = player.GetComponent<Animator>();
    }

    void Update()
    {
        // Uncomment to allow shooting input
        // if (Input.GetButtonDown("Fire1"))
        // {
        //     Shoot();
        // }

        if (Input.GetKeyDown(KeyCode.R)) // Press R to reload
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

        int needed = maxAmmo - currentAmmo;                   // Bullets required to fill cylinder
        int bulletsToLoad = Mathf.Min(needed, reserveAmmo);    // Load only what's available

        pistolAnimator.Play("ReloadRevolver_1");

        currentAmmo += bulletsToLoad;
        reserveAmmo -= bulletsToLoad;

        Debug.Log($"Reloaded! Chamber: {currentAmmo} / {maxAmmo} | Reserve: {reserveAmmo}");
        UpdateAmmoUI();
    }

    void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = $"{currentAmmo} / {reserveAmmo}";
        }
    }

    public void AddReserveAmmo(int amount)
    {
        reserveAmmo += amount;
        Debug.Log("Picked up ammo crate! Reserve now: " + reserveAmmo);
        UpdateAmmoUI();
    }
}


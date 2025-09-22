using UnityEngine;

public class PlayerAmmo : MonoBehaviour
{
    public int maxAmmo = 30;
    public int currentAmmo;

    void Start()
    {
        currentAmmo = maxAmmo; // start full
    }

    public void UseAmmo(int amount)
    {
        currentAmmo -= amount;
        currentAmmo = Mathf.Clamp(currentAmmo, 0, maxAmmo);
    }

    public bool IsOutOfAmmo()
    {
        return currentAmmo <= 0;
    }

    public void RefillAmmo()
    {
        currentAmmo = maxAmmo;
        Debug.Log("Ammo replenished!");
    }
}


using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public enum PowerUpType
{
    SpeedBoost,
    DamageIncrease,
    Invisibility,
    HealthRestore
}

public class PowerUpPad : MonoBehaviour
{
    public ParticleSystem PowerPadParticles;

    [Header("Orb Settings")]
    public GameObject powerOrb;
    public Vector3 orbOffset = new Vector3(0, 1.2f, 0);

    [Header("Cooldown Settings")]
    public float cooldown = 5f;
    private bool isAvailable = false;

    [Header("Orb Colors")]
    public Color speedBoostColor = Color.cyan;
    public Color damageIncreaseColor = Color.red;
    public Color invisibilityColor = Color.magenta;
    public Color healthRestoreColor = Color.green;

    [Header("UI Feedback")]
    public TextMeshProUGUI powerUpText;


    public float floatStrength = 0.5f;
    public float rotationSpeed = 0.0f;

    private Vector3 startPos;
    private PowerUpType currentPowerUp; // stores the randomly chosen power-up

    void Start()
    {
        startPos = transform.position;

        if (powerOrb == null)
        {
            Debug.LogError("PowerUpPad: Orb is not assigned!");
            return;
        }

        powerOrb.transform.position = transform.position + orbOffset;
        powerOrb.SetActive(false);

        StartCoroutine(RespawnOrb());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isAvailable && other.CompareTag("Player"))
        {
            Debug.Log("YOU TOUCHED THE ORB!");
            powerOrb.SetActive(false);

            if (PowerPadParticles != null)
            {
                PowerPadParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }

            ApplyPowerUp(other.gameObject);
            StartCoroutine(RespawnOrb());
        }
    }

    void Update()
    {
        if (powerOrb != null && powerOrb.activeSelf)
        {
            powerOrb.transform.position =
                transform.position + orbOffset + new Vector3(0, Mathf.Sin(Time.time) * floatStrength, 0);
            powerOrb.transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
        }
    }

    private IEnumerator RespawnOrb()
    {
        isAvailable = false;
        yield return new WaitForSeconds(cooldown);

        // Pick a random power-up each time the orb respawns
        currentPowerUp = (PowerUpType)Random.Range(0, System.Enum.GetValues(typeof(PowerUpType)).Length);
        SetOrbColor(currentPowerUp);
        powerOrb.transform.position = transform.position + orbOffset;
        powerOrb.SetActive(true);
        isAvailable = true;

        if (PowerPadParticles != null)
            PowerPadParticles.Play();

        Debug.Log($"Orb respawned with new power-up: {currentPowerUp}");
    }

    private void SetOrbColor(PowerUpType powerType)
    {
        Renderer orbRenderer = powerOrb.GetComponent<Renderer>();
        if (orbRenderer == null)
        {
            Debug.LogWarning("PowerUpPad: No Renderer found on the orb!");
            return;
        }

        Color chosenColor = Color.white; // fallback

        switch (powerType)
        {
            case PowerUpType.SpeedBoost:
                chosenColor = speedBoostColor;
                break;
            case PowerUpType.DamageIncrease:
                chosenColor = damageIncreaseColor;
                break;
            case PowerUpType.Invisibility:
                chosenColor = invisibilityColor;
                break;
            case PowerUpType.HealthRestore:
                chosenColor = healthRestoreColor;
                break;
        }

        orbRenderer.material.color = chosenColor;

        // Optional: also color the orb’s particle effect if it has one

    }

    private void ApplyPowerUp(GameObject player)
    {
        HealthBarManager healthManager = FindObjectOfType<HealthBarManager>();

        switch (currentPowerUp)
        {
            case PowerUpType.SpeedBoost:
                PlayerController playerController = player.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    StartCoroutine(ActivateSpeedBoost(playerController, 50f, 7.5f)); // +2 speed for 5 seconds
                    Debug.Log("Player picked up a Speed Boost! (+2 speed for 5 seconds)");
                }
                else
                {
                    Debug.LogWarning("No PlayerController found on player for SpeedBoost!");
                }
                break;

            case PowerUpType.DamageIncrease:
                WeaponController weapon = player.GetComponentInChildren<WeaponController>();
                if (weapon == null)
                {
                    GameObject weaponHandler = GameObject.Find("WeaponHandler");
                    if (weaponHandler != null)
                        weapon = weaponHandler.GetComponentInChildren<WeaponController>();
                }

                if (weapon != null)
                {
                    StartCoroutine(ActivateDamageBoost(weapon, 2f, 7.5f)); // 2× damage for 5 seconds
                    Debug.Log("Player picked up a Damage Boost! (Damage doubled for 5 seconds)");
                }
                else
                {
                    Debug.LogWarning("No WeaponController found in player or WeaponHandler!");
                }
                break;

            case PowerUpType.Invisibility:
                StartCoroutine(ActivateInvisibility(player, 7.5f)); // 5 seconds invisibility
                Debug.Log("Player picked up Invisibility! (Enemies temporarily disabled)");
                break;

            case PowerUpType.HealthRestore:
                if (healthManager != null)
                {
                    healthManager.AddOne();
                    Debug.Log("Player picked up a Health Restore! (+1 health bar)");
                }
                else
                {
                    Debug.LogWarning("No HealthBarManager found in the scene!");
                }
                break;
        }
        ShowPowerUpMessage(currentPowerUp);
    }

    private void ShowPowerUpMessage(PowerUpType type)
    {
        if (powerUpText == null)
        {
            Debug.LogWarning("PowerUpPad: No PowerUpText assigned!");
            return;
        }

        string message = "";
        float displayTime = 0f;

        switch (type)
        {
            case PowerUpType.SpeedBoost:
                message = "Speed Boost Activated!";
                displayTime = 7.5f;
                break;
            case PowerUpType.DamageIncrease:
                message = "Damage Boost Activated!";
                displayTime = 7.5f;
                break;
            case PowerUpType.Invisibility:
                message = "You Are Now Invisible!";
                displayTime = 7.5f;
                break;
            case PowerUpType.HealthRestore:
                message = "Health Restored!";
                displayTime = 3f;
                break;
        }

        powerUpText.text = message;
        powerUpText.gameObject.SetActive(true);


        StartCoroutine(HideTextAfterDelay(displayTime));
    }

    private IEnumerator HideTextAfterDelay(float duration)
    {
        yield return new WaitForSeconds(duration);
        if (powerUpText != null)
            powerUpText.gameObject.SetActive(false);
    }



    private IEnumerator ActivateDamageBoost(WeaponController weapon, float multiplier, float duration)
    {
        float originalDamage = weapon.GetDamage();
        weapon.SetDamage(originalDamage * multiplier);

        Debug.Log($"Damage increased from {originalDamage} to {weapon.GetDamage()} for {duration} seconds.");
        yield return new WaitForSeconds(duration);

        weapon.SetDamage(originalDamage);
        Debug.Log("Damage boost ended. Damage returned to normal.");
    }

    private IEnumerator ActivateSpeedBoost(PlayerController playerController, float boostAmount, float duration)
    {
        float originalSpeed = playerController.speed;
        playerController.speed += boostAmount;

        Debug.Log($"Speed Boost Active! Speed increased from {originalSpeed} → {playerController.speed}");
        yield return new WaitForSeconds(duration);

        playerController.speed = originalSpeed;
        Debug.Log("Speed Boost expired. Speed reverted to normal.");
    }

    private IEnumerator ActivateInvisibility(GameObject player, float duration)
    {
        NavMeshAgent[] agents = FindObjectsOfType<NavMeshAgent>();
        foreach (NavMeshAgent agent in agents)
            agent.isStopped = true;

        // Wait for duration
        yield return new WaitForSeconds(duration);

        foreach (NavMeshAgent agent in agents)
            if (agent != null)
            {
                agent.isStopped = false;
            }
    }
}

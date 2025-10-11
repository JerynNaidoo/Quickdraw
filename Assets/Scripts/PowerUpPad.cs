using UnityEngine;
using System.Collections;

public enum PowerUpType
{
    SpeedBoost,
    DamageIncrease,
    Shield,
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

    public float floatStrength = 0.5f;
    public float rotationSpeed = 2f;
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
            
            powerOrb.transform.position = transform.position + orbOffset + new Vector3(0, Mathf.Sin(Time.time) * floatStrength, 0);

            
            powerOrb.transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
        }
    }

    private IEnumerator RespawnOrb()
    {
        isAvailable = false;
        yield return new WaitForSeconds(cooldown);

        // Pick a random power-up each time the orb respawns
        currentPowerUp = (PowerUpType)Random.Range(0, System.Enum.GetValues(typeof(PowerUpType)).Length);

        powerOrb.transform.position = transform.position + orbOffset;
        powerOrb.SetActive(true);
        isAvailable = true;

        if (PowerPadParticles != null)
            PowerPadParticles.Play();

        Debug.Log($"Orb respawned with new power-up: {currentPowerUp}");
    }
    private void ApplyPowerUp(GameObject player)
    {
        HealthBarManager healthManager = FindObjectOfType<HealthBarManager>();
        
        switch (currentPowerUp)
        {
            case PowerUpType.SpeedBoost:
                Debug.Log("Player picked up a Speed Boost! (+5 speed for 2 seconds)");
                break;
            case PowerUpType.DamageIncrease:
                
                    
                    Debug.Log("Player picked up a Damage Boost!");
                
                break;
            case PowerUpType.Shield:
                Debug.Log("Player picked up a Shield! (Temporary protection active)");
                break;
            case PowerUpType.HealthRestore:
                if (healthManager != null)
                {
                    // Restore 1 bar, or more if you want (e.g., 2)
                    healthManager.AddOne();
                    Debug.Log("Player picked up a Health Restore! (+1 health bar)");
                }
                else
                {
                    Debug.LogWarning("No HealthBarManager found in the scene!");
                }
                break;
        }
    }

}




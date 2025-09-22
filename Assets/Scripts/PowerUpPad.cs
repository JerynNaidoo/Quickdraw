using UnityEngine;
using System.Collections;

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


        powerOrb.transform.position = transform.position + orbOffset;
        powerOrb.SetActive(true);
        isAvailable = true;

        if (PowerPadParticles != null)
        {
            PowerPadParticles.Play();
        }
    }
}

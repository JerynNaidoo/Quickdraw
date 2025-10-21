using UnityEngine;
using System.Collections;

public class Dynamite : MonoBehaviour
{
    [Header("Explosion Settings")]
    [SerializeField] private float fuseTime = 5f;              // Time before explosion
    [SerializeField] private float explosionRadius = 50f;       // Radius of explosion effect
    [SerializeField] private float explosionForce = 700f;      // Force applied to nearby objects
    [SerializeField] private GameObject explosionEffect;       // Particle effect prefab
    [SerializeField] private AudioClip explosionSound;         // Optional sound clip

    private bool exploded = false;

    private void Start()
    {
        // Start the fuse timer
        StartCoroutine(FuseTimer());
    }

    private IEnumerator FuseTimer()
    {
        
        yield return new WaitForSeconds(fuseTime);
        Explode();
    }

    private void Explode()
    {
        if (exploded) return;
        exploded = true;

        Debug.Log("💥 Dynamite exploded!");

        
        if (explosionEffect != null)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

        
        if (explosionSound != null)
            AudioSource.PlayClipAtPoint(explosionSound, transform.position);


        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider nearby in colliders)
        {
            // Ignore the dynamite itself
            if (nearby.gameObject == gameObject) continue;
            Debug.Log("Explosion hit: " + gameObject.name);

            // Only affect player
            if (nearby.CompareTag("Player"))
            {
                HealthBarManager health = FindObjectOfType<HealthBarManager>();
                if (health != null)
                {
                    health.RemoveOne();
                    Debug.Log("Player hit by explosion!");
                }
                else
                {
                    Debug.LogWarning("No HealthBarManager found on player!");
                }
            }
        }




        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        // Draw explosion radius in Scene view
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}

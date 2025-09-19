using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    private NavMeshAgent agent;

    [Header("Ragdoll Settings")]
    public GameObject enemyBody; // Assign the EnemyBody child object in Inspector
    private Rigidbody enemyBodyRb;
    private Collider enemyBodyCollider;

    [Header("Stats")]
    private float currentHealth = 100f;

    [Header("Death Settings")]
    public float despawnTime = 2f;
    private bool isDead = false;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();

        if (enemyBody != null)
        {
            enemyBodyRb = enemyBody.GetComponent<Rigidbody>();
            enemyBodyCollider = enemyBody.GetComponent<Collider>();

            if (enemyBodyRb != null)
            {
                // Keep Rigidbody active for collisions with world
                enemyBodyRb.isKinematic = false;
                enemyBodyRb.useGravity = true;

                // Prevent falling over while alive
                enemyBodyRb.constraints = RigidbodyConstraints.FreezeRotationX |
                                          RigidbodyConstraints.FreezeRotationZ;
            }
        }
    }

    private void Update()
    {
        if (!isDead)
            ChasePlayer();
    }

    private void ChasePlayer()
    {
        if (player != null && agent != null && agent.enabled && agent.isOnNavMesh)
        {
            agent.SetDestination(player.position);
        }
    }

    public void TakeDamage(float amount)
    {
        if (!isDead)
        {
            currentHealth -= amount;
            Debug.Log(gameObject.name + " took " + amount + " damage. Remaining HP: " + currentHealth);

            if (currentHealth <= 0f)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        isDead = true;
        agent.enabled = false; // stop NavMesh movement

        if (enemyBodyRb != null)
        {
            // Release rigidbody so ragdoll works
            enemyBodyRb.constraints = RigidbodyConstraints.None;
            enemyBodyRb.useGravity = true;

            // Knockback force
            enemyBodyRb.AddForce(-transform.forward * 5f + Vector3.up * 2f, ForceMode.Impulse);
        }

        EnemySpawner spawner = Object.FindFirstObjectByType<EnemySpawner>();
        if (spawner != null)
            spawner.EnemyDied();

        Destroy(gameObject, despawnTime);
    }
}


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
    [SerializeField] private float currentHealth = 100f; // Made SerializeField so it can be adjusted in Inspector

    [Header("Death Settings")]
    public float despawnTime = 2f;
    private bool isDead = false;
    [SerializeField] private float damageCooldown = 1.0f;
    private float lastDamageTime;

    private void Awake()
    {
        if (player == null)
            player = GameObject.Find("Player")?.transform;

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
        if (isDead) return;

        currentHealth -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage. Remaining HP: {currentHealth}");

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        agent.enabled = false; // stop NavMesh movement

        // Enable ragdoll
        if (enemyBodyRb != null)
        {
            enemyBodyRb.constraints = RigidbodyConstraints.None;
            enemyBodyRb.useGravity = true;

            // Optional knockback force
            enemyBodyRb.AddForce(-transform.forward * 5f + Vector3.up * 2f, ForceMode.Impulse);
        }

        // Notify spawner
        EnemySpawner spawner = Object.FindFirstObjectByType<EnemySpawner>();
        if (spawner != null)
        {
            spawner.EnemyDied();
        }
        else
        {
            Debug.LogWarning("EnemySpawner not found in scene!");
        }

        // Destroy the enemy object after a delay
        Destroy(gameObject, despawnTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && Time.time - lastDamageTime > damageCooldown)
        {
            // Damage the player (replace this with your own health system)
            HealthBarManager health = Object.FindFirstObjectByType<HealthBarManager>();
            if (health != null)
                health.RemoveOne();

            lastDamageTime = Time.time;
        }
    }
}

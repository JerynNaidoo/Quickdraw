using UnityEngine;
using UnityEngine.AI;



public class Enemy : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    private NavMeshAgent agent;
    private Rigidbody rb;

    [Header("Stats")]
    private float currentHealth = 100f;

    [Header("Death Settings")]
    public float despawnTime = 2f; // time after death before enemy is removed
    private bool isDead = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;   // NavMeshAgent drives movement
        rb.useGravity = false;   // no physics gravity while alive
        rb.constraints = RigidbodyConstraints.FreezeAll; // completely locked

    }

    // Update is called once per frame
    void Update()
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
            Debug.Log(gameObject.name + " took " +amount +"damage. Remaining HP: " + currentHealth);

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
        rb.freezeRotation = false; // allow ragdoll-like physics
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.None; // free ragdoll physics

        // add force to make it fall dramatically
        rb.AddForce(Vector3.back * 5f, ForceMode.Impulse);

        // Tell spawner this enemy is gone
        EnemySpawner spawner = Object.FindFirstObjectByType<EnemySpawner>(); ;
        if (spawner != null) 
            spawner.EnemyDied();
        Debug.Log("Agent enabled: " + agent.enabled + " | isOnNavMesh: " + agent.isOnNavMesh);
        Destroy(gameObject, despawnTime);
    }
}

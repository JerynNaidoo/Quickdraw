using UnityEngine;
using UnityEngine.AI;

public class Boss : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    [Header("Death Settings")]
    [SerializeField] private float despawnTime = 2f;
    private bool isDead = false;

    public EndGameCinematic endCinematic;

    private Transform player;
    private NavMeshAgent agent;
    private Animator animator;


    private void Awake()
    {
        currentHealth = maxHealth;
        if (player == null)
            player = GameObject.Find("Player")?.transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    /// <summary>
    /// Call this to apply damage to the enemy.
    /// </summary>
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


    private void Update()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player")?.transform;
            if (player == null)
                return; // still not found, skip movement this frame
        }

        if (!isDead && agent != null && agent.isOnNavMesh)
            agent.SetDestination(player.position);
    }

    //private void ChasePlayer()
    //{
    //    if (player == null)
    //    {
    //        Debug.LogWarning("No player found!");
    //        return;
    //    }

    //    if (agent == null)
    //    {
    //        Debug.LogWarning("No NavMeshAgent found!");
    //        return;
    //    }

    //    if (!agent.isOnNavMesh)
    //    {
    //        Debug.LogWarning("Boss is not on NavMesh!");
    //        return;
    //    }
    //    if (player != null && agent != null && agent.enabled && agent.isOnNavMesh)
    //    {
    //        agent.SetDestination(player.position);
    //    }
    //}


    private void Die()
    {
        isDead = true;
        Debug.Log($"{gameObject.name} died!");

        if (endCinematic != null)
        {
            endCinematic.StartCinematic();
        }


        Destroy(gameObject, despawnTime);
        
    }
}

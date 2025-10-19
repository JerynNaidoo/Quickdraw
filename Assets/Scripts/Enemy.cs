using System.Collections;
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

    private Animator animator;

    // Diagnostics / auto-resolve
    private int lastLoggedFrame = -1;
    private float lastLogTime = -10f;
    [SerializeField] private float logIntervalSec = 0.5f;
    private Vector3 lastObservedPlayerPos = Vector3.positiveInfinity;
    private int lastObservedPlayerInstanceId = -1;
    [SerializeField] private int stalePositionFrameThreshold = 30; // frames before we warn a position seems stale
    private int framesWithSamePlayerPos = 0;

    private void Awake()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player")?.transform;

        agent = GetComponent<NavMeshAgent>();

        if (enemyBody != null)
        {
            enemyBodyRb = enemyBody.GetComponent<Rigidbody>();
            enemyBodyCollider = enemyBody.GetComponent<Collider>();
            animator = GetComponentInChildren<Animator>();

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

        if (player != null)
        {
            lastObservedPlayerPos = player.position;
            lastObservedPlayerInstanceId = player.gameObject.GetInstanceID();
        }
    }

    private void Update()
    {
        // Auto-resolve if player reference was replaced in the scene
        Transform fresh = GameObject.FindWithTag("Player")?.transform;
        if (fresh != null && player != fresh)
        {
            //Debug.Log($"[Enemy:{name}] Detected different Player instance. Reassigning player from id {lastObservedPlayerInstanceId} to id {fresh.gameObject.GetInstanceID()} (name '{fresh.name}').");
            player = fresh;
            lastObservedPlayerPos = player.position;
            lastObservedPlayerInstanceId = player.gameObject.GetInstanceID();
            framesWithSamePlayerPos = 0;
        }

        if (!isDead)
            ChasePlayer();

        // Throttled diagnostic showing exact instance and frame
        if (player != null && Time.unscaledTime - lastLogTime > logIntervalSec)
        {
            int curInstance = player.gameObject.GetInstanceID();
            Vector3 curPos = player.position;
            int frame = Time.frameCount;
            //Debug.Log($"[Enemy:{name}] player.position={curPos} (player.name='{player.name}', id={curInstance}) frame={frame} time={Time.time:F2}s agent.isOnNavMesh={(agent != null ? agent.isOnNavMesh : false)}");

            // detect stale referenced Transform (position not changing on the referenced Transform)
            if (curPos == lastObservedPlayerPos)
            {
                framesWithSamePlayerPos++;
                if (framesWithSamePlayerPos > stalePositionFrameThreshold)
                {
                    //Debug.LogWarning($"[Enemy:{name}] Player Transform (id={curInstance}) position unchanged for {framesWithSamePlayerPos} frames — may be a stale/incorrect Transform reference (child/camera) or player is being moved elsewhere. Resolved player.root='{player.root.name}' active={player.gameObject.activeInHierarchy}.");
                }
            }
            else
            {
                framesWithSamePlayerPos = 0;
            }

            lastObservedPlayerPos = curPos;
            lastObservedPlayerInstanceId = curInstance;
            lastLogTime = Time.unscaledTime;
            lastLoggedFrame = frame;
        }

        // Visual debug: draw line to the currently referenced Transform
        if (player != null)
            Debug.DrawLine(transform.position, player.position, Color.red, 0.1f);
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

            if (animator != null)
            {
                animator.SetBool("canAttack", true);
                StartCoroutine(ResetAttackAfterDelay(2f));
            }
        }
    }

    private IEnumerator ResetAttackAfterDelay(float delay)
    {
        //Debug.Log("Coroutine started: will reset attack in " + delay + " seconds");
        yield return new WaitForSeconds(delay);
        if (animator != null) animator.SetBool("canAttack", false);
        //Debug.Log("canAttack reset to false");
    }

}

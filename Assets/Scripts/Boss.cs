using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class Boss : MonoBehaviour
{
    public TextMeshProUGUI messageText;
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
    private Collider BossCollider;

    [Header("Attack Settings")]
    [SerializeField] private float damageCooldown = 1.0f;
    private float lastDamageTime = 0;

    [Header("Throwing Settings")]
    [SerializeField] private GameObject dynamitePrefab;
    [SerializeField] private Transform throwPoint;
    [SerializeField] private float throwForce = 10f;
    [SerializeField] private float throwCooldown = 5f;
    private float lastThrowTime = 0f;
    private float messageTimer = 0f;
    private bool messageActive = false;




    private void Awake()
    {
        

        currentHealth = maxHealth;
        if (player == null)
            player = GameObject.Find("Player")?.transform;

        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        BossCollider = GetComponent<Collider>();
        animator.SetBool("isRunning", true);
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

    private void Update()
    {
        // Find player if missing
        if (player == null)
        {
            player = GameObject.FindWithTag("Player")?.transform;
            if (player == null) return;
        }

        // Move toward player
        if (!isDead && agent != null && agent.isOnNavMesh)
            agent.SetDestination(player.position);

        // Animator speed (optional)
        //if (agent != null && animator != null)
        //{
        //    float currentSpeed = agent.velocity.magnitude;
        //    animator.SetFloat("speed", currentSpeed);
        //}

        // Trigger throw if cooldown passed and player is far enough
        if (!isDead && Time.time - lastThrowTime > throwCooldown && Vector3.Distance(transform.position, player.position) > 5f)
        {
            TriggerThrow();
            lastThrowTime = Time.time;
        }

        // Handle message timer
        if (messageActive)
        {
            messageTimer -= Time.deltaTime;
            if (messageTimer <= 0f)
            {
                if (messageText != null)
                    messageText.text = "";
                messageActive = false;
            }
        }
    }

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

    private void OnTriggerEnter(Collider other)
    {
        if (isDead) return;

        if (other.CompareTag("Player") && Time.time - lastDamageTime > damageCooldown)
        {
            Debug.Log("Boss collided with Player — applying damage.");
            HealthBarManager health = Object.FindFirstObjectByType<HealthBarManager>();
            if (health != null)
                health.RemoveOne();

            lastDamageTime = Time.time;
            animator.SetBool("canAttack", true);
            StartCoroutine(ResetAttackAfterDelay(2f));
        }
    }

    private IEnumerator ResetAttackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        animator.SetBool("canAttack", false);
    }

    public void TriggerThrow()
    {
        if (isDead || animator == null) return;

        Debug.Log("Triggering Throw animation");

        if (messageText != null)
        {
            messageText.text = "Dynamite Incoming";
            messageTimer = 2f;    // show for 1 second
            messageActive = true; // start countdown
        }

        animator.SetTrigger("Throw");
    }







    // This method should be called via an Animation Event
    public void ThrowDynamite()
    {
        //if (dynamitePrefab == null || throwPoint == null || player == null) return;

        //GameObject dynamite = Instantiate(dynamitePrefab, throwPoint.position, Quaternion.identity);
        //Rigidbody rb = dynamite.GetComponent<Rigidbody>();
        //if (rb != null)
        //{
        //    Vector3 direction = (player.position - throwPoint.position).normalized;
        //    rb.AddForce(direction * throwForce, ForceMode.VelocityChange);
        //}

        if (dynamitePrefab == null || player == null) return;

        // Start a little above the player
        Vector3 spawnPos = player.position + player.forward * 1.5f;
        spawnPos.y = player.position.y; // lock to player’s ground height
        Instantiate(dynamitePrefab, spawnPos, Quaternion.identity);



    }

    public void SpawnDynamite()
    {

        if (dynamitePrefab == null || player == null) return;

        // Spawn position: a little in front of the player’s current position
        Vector3 spawnOffset = player.forward * 1.5f + Vector3.up * 0.5f;
        Vector3 spawnPos = player.position + spawnOffset;

        // Spawn the dynamite prefab facing the same direction as the player
        Instantiate(dynamitePrefab, spawnPos, Quaternion.LookRotation(player.forward));


        Debug.Log("Dynamite spawned in front of the player!");
    }


}
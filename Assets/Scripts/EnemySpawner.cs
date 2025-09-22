using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject enemyPrefab;   // The enemy prefab to spawn
    public int maxEnemies = 10;      // Limit how many enemies can exist
    public int currentEnemies = 0;
    private int enemiesSpawned = 0;
    public TextMeshProUGUI enemyCountText;

    [Header("Spawn Settings")]
    public float spawnInterval = 3f; // Time between spawns
    public Transform[] spawnPoints;  // Where enemies spawn

    [Header("References")]
    public Transform player;           // Assign the player in the inspector
    public EndFight endScreen; // assign the local panel
    public Vector3 finalBattlePosition; // Where to move player for final battle

    [Header("Final Battle Settings")]
    public int killsRequiredForFinal = 10;

    private int enemiesKilled = 0;

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private System.Collections.IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (enemiesSpawned < maxEnemies)
            {
                SpawnEnemy();
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab == null || spawnPoints.Length == 0 || player == null) return;

        // Pick a random spawn point
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Instantiate the enemy
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

        // Assign player reference to EnemyMovement
        Enemy movement = enemy.GetComponent<Enemy>();
        if (movement != null)
        {
            movement.player = player;
            
        }

        // Warp agent onto NavMesh
        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(spawnPoint.position, out hit, 2f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
            }
        }

        currentEnemies++;
        enemiesSpawned++;
        UpdateEnemyCountDisplay();
        Debug.Log($"Spawned enemy. Current enemies: {currentEnemies}");
    }

    // Call this from the Enemy script when it dies
    public void EnemyDied()
    {
        currentEnemies--;
        enemiesKilled++;
        UpdateEnemyCountDisplay();

        // Check if the final battle trigger is met
        if (enemiesKilled >= killsRequiredForFinal)
        {
            StartCoroutine(TriggerFinalBattleSequence());
        }
    }

    private void UpdateEnemyCountDisplay()
    {
        if (enemyCountText != null)
        {
            enemyCountText.text = $"{currentEnemies}";
        }
    }

    private System.Collections.IEnumerator TriggerFinalBattleSequence()
    {
        // Delay 2 seconds
        yield return new WaitForSeconds(2f);

        // Show first message
        if (endScreen != null)
            endScreen.ShowMessage("You proved worthy of challenge. I have to take it upon my hands.");
        

        yield return new WaitForSeconds(2f);

        // Show prepare message
        if (endScreen != null)
            endScreen.ShowMessage("Prepare yourself for the final battle!");


        // Move the player
        if (player != null)
            player.position = finalBattlePosition;

        // Optional: hide the black panel after a short delay
        yield return new WaitForSeconds(1f);
        if (endScreen != null && endScreen.blackPanel != null)
            endScreen.blackPanel.SetActive(false);
    }
}

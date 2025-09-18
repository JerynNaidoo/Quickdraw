using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject enemyPrefab;   // The enemy prefab to spawn
    public int maxEnemies = 10;      // Limit how many enemies can exist
    private int currentEnemies = 0;

    [Header("Spawn Settings")]
    public float spawnInterval = 3f; // Time between spawns
    public Transform[] spawnPoints;  // Where enemies spawn

    [Header("References")]
    public Transform player; // Assign the player in the inspector

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private System.Collections.IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (currentEnemies < maxEnemies)
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

        // Warp agent onto NavMesh so it doesn't float
        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.Warp(enemy.transform.position); // snaps agent to nearest point on NavMesh
        }

        currentEnemies++;
    }

    public void EnemyDied()
    {
        currentEnemies--;
    }
}

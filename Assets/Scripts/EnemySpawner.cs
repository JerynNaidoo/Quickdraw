using UnityEngine;
using UnityEngine.AI;
using TMPro;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject enemyPrefab;       // Enemy prefab
    public TextMeshProUGUI enemyCountText;

    [Header("Spawn Settings")]
    public Transform[] spawnPoints;      // Spawn locations
    public Transform player;             // Player reference
    public float spawnInterval = 1.5f;   // Time between spawns

    [Header("Wave Settings")]
    public int totalWaves = 3;           // Total waves
    public int enemiesInFirstWave = 2;   // Starting enemies per wave
    public float waveInterval = 15f;     // Delay between waves

    [Header("Final Battle Settings")]
    public EndFight endFight;            // Cinematic system
    public Vector3 finalBattlePosition;  // Player position for final battle

    [Header("Difficulty Scaling")]
    public float enemyHealthIncrease = 20f; // Extra HP per wave

    private int currentWave = 0;
    private int enemiesAlive = 0;
    private Coroutine waveCoroutine;
    private bool finalBattleTriggered = false;

    private void Start()
    {
        currentWave = 0;
        enemiesAlive = 0;
        finalBattleTriggered = false;
        waveCoroutine = StartCoroutine(WaveRoutine());
    }

    private IEnumerator WaveRoutine()
    {
        yield return new WaitForSeconds(2f); // Initial delay


        while (currentWave < totalWaves)
        {
            currentWave++; 
            int enemiesToSpawn = enemiesInFirstWave + (currentWave - 1) * 3;

            Debug.Log($"--- Wave {currentWave}/{totalWaves} ({enemiesToSpawn} enemies) ---");

            // Show wave start message
            if (endFight != null)
            {
                endFight.ShowMessage($"Wave {currentWave} incoming!");
                yield return new WaitForSeconds(2f);
                endFight.Hide();
            }

            // Spawn enemies
            for (int i = 0; i < enemiesToSpawn; i++)
            {
                SpawnEnemy(currentWave);
                yield return new WaitForSeconds(spawnInterval);
            }

            // Wait until all enemies are dead
            while (enemiesAlive > 0)
                yield return null;

            Debug.Log($"Wave {currentWave} cleared!");

            // Show wave cleared message except for final wave
            if (currentWave < totalWaves && endFight != null)
            {
                endFight.ShowMessage($"Wave {currentWave} cleared! Next wave in {waveInterval} seconds...");
                yield return new WaitForSeconds(waveInterval);
                endFight.Hide();
            }

            
        }
        if (!finalBattleTriggered)
        {
            finalBattleTriggered = true;
            StartCoroutine(TriggerFinalBattleSequence());
        }

        // Trigger final battle

    }

    private void SpawnEnemy(int wave)
    {
        if (enemyPrefab == null || spawnPoints.Length == 0 || player == null) return;

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

        Enemy movement = enemy.GetComponent<Enemy>();
        if (movement != null)
        {
            movement.player = player;
            float newHealth = 100f + (enemyHealthIncrease * (wave - 1));
            typeof(Enemy)
                .GetField("currentHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(movement, newHealth);
        }

        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(spawnPoint.position, out hit, 2f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
            }
        }

        enemiesAlive++;
        UpdateEnemyCountDisplay();

        Debug.Log($"Spawned enemy. Enemies alive: {enemiesAlive}");
    }

    // Called by Enemy script when it dies
    public void EnemyDied()
    {
        enemiesAlive = Mathf.Max(0, enemiesAlive - 1);
        UpdateEnemyCountDisplay();
        Debug.Log($"Enemy died! Enemies alive: {enemiesAlive}");
    }

    private void UpdateEnemyCountDisplay()
    {
        if (enemyCountText != null)
            enemyCountText.text = $"Enemies: {Mathf.Max(enemiesAlive, 0)}";
    }

    private IEnumerator TriggerFinalBattleSequence()
    {
        Debug.Log("Starting final battle cinematic...");
        //yield return new WaitForSeconds(1f);

        //// Play cinematic
        //if (endFight != null)
        //{
        //    StartCoroutine(endFight.PlayFinalSequence());
        //}
        //else
        //{
        //    Debug.LogWarning("EndFight reference not assigned!");
        //}

        //// Move player to final battle position
        //if (player != null)
        //{
        //    player.position = finalBattlePosition;
        //    Debug.Log($"Player moved to final battle arena at {finalBattlePosition}");
        //}


        yield return StartCoroutine(endFight.PlayFinalSequence());
        if (player != null)
        {
            player.position = finalBattlePosition;
            Debug.Log($"Player moved to final battle arena at {finalBattlePosition}");
        }

    }
}

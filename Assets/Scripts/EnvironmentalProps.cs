using UnityEngine;
using System.Collections.Generic;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject[] objectsToSpawn;      
    public int numberOfObjects = 100;        
    public float minDistance = 2f;           
    public Vector2 areaSize = new Vector2(800, 800); 

    private List<Vector3> spawnedPositions = new List<Vector3>();

    void Start()
    {
        SpawnObjects();
    }

    float GetHeightAtPosition(Vector3 pos)
    {
        foreach (Terrain t in Terrain.activeTerrains)
        {
            Vector3 tPos = t.transform.position;
            Vector3 tSize = t.terrainData.size;

            if (pos.x >= tPos.x && pos.x <= tPos.x + tSize.x &&
                pos.z >= tPos.z && pos.z <= tPos.z + tSize.z)
            {
                return t.SampleHeight(pos);
            }
        }
        return 0f;
    }

    void SpawnObjects()
    {
        int attempts = 0;
        int maxAttempts = numberOfObjects * 20; 

        while (spawnedPositions.Count < numberOfObjects && attempts < maxAttempts)
        {
            attempts++;

            float randomX = Random.Range(-areaSize.x / 2f, areaSize.x / 2f);
            float randomZ = Random.Range(-areaSize.y / 2f, areaSize.y / 2f);

            Vector3 spawnPos = new Vector3(randomX, 0, randomZ) + transform.position;
            float terrainHeight = GetHeightAtPosition(spawnPos);
            spawnPos.y = terrainHeight;

            // Check minimum distance
            bool tooClose = false;
            foreach (Vector3 pos in spawnedPositions)
            {
                if (Vector3.Distance(spawnPos, pos) < minDistance)
                {
                    tooClose = true;
                    break;
                }
            }
            if (tooClose) continue;

            
            GameObject prefab = objectsToSpawn[Random.Range(0, objectsToSpawn.Length)];
            Quaternion rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

            Instantiate(prefab, spawnPos, rotation);
            spawnedPositions.Add(spawnPos);
        }
    }
}

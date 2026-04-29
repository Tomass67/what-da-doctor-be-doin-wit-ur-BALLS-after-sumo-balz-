using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject enemy2Prefab;
    public GameObject bossPrefab; // NEW boss
    public GameObject powerupPrefab;

    private float spawnRange = 9;

    public int enemyCount;
    public int waveNumber = 1;

    void Start()
    {
        Instantiate(powerupPrefab, GenerateSpawnPosition(), powerupPrefab.transform.rotation);
        SpawnEnemyWave(waveNumber);
    }

    void Update()
    {
        enemyCount = FindObjectsOfType<Enemy>().Length;

        if (enemyCount == 0)
        {
            waveNumber++;
            Instantiate(powerupPrefab, GenerateSpawnPosition(), powerupPrefab.transform.rotation);
            SpawnEnemyWave(waveNumber);
        }
    }

    void SpawnEnemyWave(int enemiesToSpawn)
    {
        // 🔥 Boss wave every 3 waves
        if (waveNumber % 3 == 0)
        {
            Instantiate(bossPrefab, GenerateSpawnPosition(), bossPrefab.transform.rotation);
        }

        // Spawn normal enemies too
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            GameObject enemyToSpawn;

            if (waveNumber >= 2 && Random.value > 0.5f)
            {
                enemyToSpawn = enemy2Prefab;
            }
            else
            {
                enemyToSpawn = enemyPrefab;
            }

            Instantiate(enemyToSpawn, GenerateSpawnPosition(), enemyToSpawn.transform.rotation);
        }
    }

    Vector3 GenerateSpawnPosition()
    {
        float spawnPosX = Random.Range(-spawnRange, spawnRange);
        float spawnPosZ = Random.Range(-spawnRange, spawnRange);
        return new Vector3(spawnPosX, 0, spawnPosZ);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public int currentWaveCount;
    public float waveInterval;
    public List<Wave> waves;
    public List<Transform> spawnPoints;

    float spawnTimer;
    public int enemiesAlive;
    public int maxEnemiesAllowed;
    public bool maxEnemiesReached;

    Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        CalculateWaveQuota();
    }

    void Update()
    {
        if(currentWaveCount < waves.Count && waves[currentWaveCount].spawnCount == 0){
            StartCoroutine(BeginNextWave());
        }

        spawnTimer += Time.deltaTime;

        if(spawnTimer >= waves[currentWaveCount].spawnInterval){
            spawnTimer = 0f;
            SpawnEnemies();
        }
    }
    
    void CalculateWaveQuota()
    {
        int currentWaveQuota = 0;
        foreach (var enemyGroup in waves[currentWaveCount].enemies){
            currentWaveQuota += enemyGroup.enemyCount;
        }

        waves[currentWaveCount].waveQuota = currentWaveQuota;
    }

    void SpawnEnemies()
    {
        if(waves[currentWaveCount].spawnCount < waves[currentWaveCount].waveQuota &&
            !maxEnemiesReached){
            foreach (var enemyGroup in waves[currentWaveCount].enemies){
                if(enemyGroup.spawnCount < enemyGroup.enemyCount){
                    if(enemiesAlive >= maxEnemiesAllowed){
                        maxEnemiesReached = true;
                        return;
                    }

                    var randomPoint = Random.Range(0, spawnPoints.Count);
                    EnemyController enemy = Instantiate(enemyGroup.enemyPrefab, player.position + spawnPoints[randomPoint].position, Quaternion.identity);
                    enemy.transform.SetParent(spawnPoints[randomPoint]);

                    // Vector3 spawnPosition = new Vector3(player.transform.position.x + Random.Range(-10f, 10f), 
                    //                                     0f, player.transform.position.z + Random.Range(-10f, 10f));
                    // Instantiate(enemyGroup.enemyPrefab, spawnPosition, Quaternion.identity);

                    enemyGroup.spawnCount++;
                    waves[currentWaveCount].spawnCount++;
                    enemiesAlive++;
                }
            }
        }

        if(enemiesAlive < maxEnemiesAllowed){ maxEnemiesReached = false; }
    }

    public void EnemyKilled(){ enemiesAlive--; }

    IEnumerator BeginNextWave()
    {
        yield return new WaitForSeconds(waveInterval);

        if(currentWaveCount < waves.Count - 1){
            currentWaveCount++;
            CalculateWaveQuota();
        }
    }
}

[System.Serializable]
public class Wave
{
    public List<EnemyGroup> enemies;
    public int waveQuota;
    public float spawnInterval;

    public int spawnCount;             
}

[System.Serializable]
public class EnemyGroup
{
    public int enemyCount;
    public int spawnCount;
    public EnemyController enemyPrefab;
}

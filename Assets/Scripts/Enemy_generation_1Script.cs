using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnScript : MonoBehaviour
{
    // Variable to store the enemy prefab
    public GameObject enemy;
    // Number of enemies to spawn in the current wave
    private int enemiesToSpawn = 1;
    // Time interval between enemy spawns within a wave
    private float spawnInterval = 2;
    // Current wave number
    private int waveNumber = 1;
    // Variable to keep track of spawned enemies in the current wave
    private int spawnedEnemies = 0;

    // Start is called before the first frame update
    void Start()
    {
        StartNextWave();
    }

    // Start the next wave
    void StartNextWave()
    {
        // Set the number of enemies to spawn in the current wave
        enemiesToSpawn = waveNumber;
        // Reset the spawned enemies count
        spawnedEnemies = 0;
        // Start spawning enemies within the wave
        StartCoroutine(SpawnEnemies());
    }

    // Coroutine for spawning enemies within a wave
    IEnumerator SpawnEnemies()
    {
        float randomValue = Random.Range(0f, 1f);
        yield return new WaitForSeconds(randomValue);

        while (spawnedEnemies < enemiesToSpawn)
        {
            SpawnEnemy();
            spawnedEnemies++;
            yield return new WaitForSeconds(spawnInterval);
        }

        // If all enemies in the wave are spawned and all enemies are destroyed, start the next wave
        yield return new WaitUntil(() => spawnedEnemies == enemiesToSpawn && GameObject.FindGameObjectsWithTag("enemy").Length == 0);

        if (waveNumber < 2)
        {
            waveNumber++;
            StartNextWave();
        }
    }

    // Spawn a single enemy
    void SpawnEnemy()
    {
        Vector2 spawnPoint = new Vector2(transform.position.x, transform.position.y);

        // Create an enemy at the 'spawnPoint' position
        Instantiate(enemy, spawnPoint, Quaternion.identity);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavesManager : MonoBehaviour
{
    [System.Serializable]
    public class EnemyType
    {
        public GameObject enemyPrefab;
        public float spawnChance = 100;
    }

    [System.Serializable]
    public class Wave
    {
        public List<EnemyType> enemyTypes;
        public int baseEnemyCount;
        public float spawnInterval;
        public float waveDuration;
    }

    [Header("Wave Configuration")]
    public List<Wave> waves;
    public List<Transform> spawnPoints;
    public float timeBetweenWaves = 5.0f;

    private int currentWaveIndex = 0;
    private int nbEnemies = 0;
    private float waveTimer;
    private bool wavesEnded = false;
    private bool isSpawning = false;

    private Transform enemiesTarget;

    void Start()
    {
        if (waves.Count > 0)
        {
            StartCoroutine(StartNextWave());
        }
        enemiesTarget = transform;
    }

    void Update()
    {
        if (isSpawning)
        {
            waveTimer -= Time.deltaTime;
            if (waveTimer <= 0)
            {
                isSpawning = false;
                if (currentWaveIndex < waves.Count - 1)
                {
                    currentWaveIndex++;
                    StartCoroutine(StartNextWave());
                }
                else if (wavesEnded)
                {
                    FinishWaves();
                }
            }
        }
    }

    IEnumerator StartNextWave()
    {
        wavesEnded = false;
        Wave currentWave = waves[currentWaveIndex];
        Debug.Log(currentWaveIndex);
        nbEnemies = currentWave.baseEnemyCount;
        waveTimer = currentWave.waveDuration;
        isSpawning = true;

        yield return new WaitForSeconds(timeBetweenWaves);
        Debug.Log(isSpawning);
        while (nbEnemies > 0 && isSpawning)
        {
            SpawnEnemy(currentWave);
            yield return new WaitForSeconds(currentWave.spawnInterval);
        }

        wavesEnded = true;
    }

    void SpawnEnemy(Wave wave)
    {
        if (wave.enemyTypes.Count == 0)
        {
            return;
        }

        float totalChance = 0;
        foreach (var enemyType in wave.enemyTypes)
        {
            totalChance += enemyType.spawnChance;
        }

        float randomValue = Random.Range(0, totalChance);
        float cumulativeChance = 0;
        EnemyType selectedEnemy = wave.enemyTypes[0];

        foreach (var enemyType in wave.enemyTypes)
        {
            cumulativeChance += enemyType.spawnChance;
            if (randomValue <= cumulativeChance)
            {
                selectedEnemy = enemyType;
                break;
            }
        }

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

        GameObject enemy = Instantiate(selectedEnemy.enemyPrefab, spawnPoint.position, Quaternion.identity);
        enemy.transform.parent = transform;
        enemy.SetActive(true);

        SampleEnemy enemyComponent = enemy.GetComponent<SampleEnemy>();
        if (enemyComponent != null)
        {
            enemyComponent.Initialize(enemiesTarget);
        }

        nbEnemies--;
    }

    void FinishWaves()
    {
        Debug.Log("Cristal activated!");
        // Ajoutez ici des actions à effectuer une fois toutes les vagues terminées.
    }
}

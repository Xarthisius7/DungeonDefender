using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

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
    public float timeBetweenWaves = 5.0f;

    private List<Transform> spawnPoints;
    private int currentWaveIndex = 0;
    private int nbEnemies = 0;
    private float waveTimer;
    private bool wavesEnded = false;
    private bool isSpawning = false;
    private Animator animator;
    private Transform enemiesTarget;
    private List<Powerup> allPowerups;

    [Header("Cristal Configuration")]
    private Sprite defaultSprite;
    private SpriteRenderer spriteRenderer;
    private Light2D crystalLight;
    private float lightIntensity;

    void Start()
    {
        InitSpawPoints();
        if (waves.Count > 0)
        {
            StartCoroutine(StartNextWave());
        }
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultSprite = spriteRenderer.sprite;
        animator = GetComponent<Animator>();
        crystalLight = GetComponent<Light2D>();
        lightIntensity = crystalLight.intensity;
        if (animator == null)
            Debug.Log("no animator found");
        else {
            spriteRenderer.sprite = defaultSprite;
            crystalLight.intensity = 0.5f;
            animator.enabled = false;
        }
        allPowerups = new List<Powerup>();
    }

    void InitSpawPoints()
    {
        spawnPoints = new List<Transform>();

        GameObject[] spawnPointObjects = GameObject.FindGameObjectsWithTag("EnemieSpawnPoint");

        foreach (GameObject obj in spawnPointObjects)
        {
            spawnPoints.Add(obj.transform);
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
        if (animator == null)
            Debug.Log("no animator found");
        else {
            crystalLight.intensity = 1.5f;
            animator.enabled = true;
        }
        wavesEnded = false;
        Wave currentWave = waves[currentWaveIndex];
        nbEnemies = currentWave.baseEnemyCount;
        waveTimer = currentWave.waveDuration;
        isSpawning = true;

        yield return new WaitForSeconds(timeBetweenWaves);
        while (nbEnemies > 0 && isSpawning)
        {
            SpawnEnemy(currentWave);
            yield return new WaitForSeconds(currentWave.spawnInterval);
        }

        if (animator == null)
            Debug.Log("no animator found");
        else{
            animator.enabled = false;
            spriteRenderer.sprite = defaultSprite;
            crystalLight.intensity = 0.5f;
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

        SampleEnemy enemyComponent = enemy.GetComponentInChildren<SampleEnemy>();
        if (enemyComponent != null)
        {
            enemyComponent.Initialize(enemiesTarget);
        }
        else
        {
            Debug.Log("enemyComponent not found");
        }
        nbEnemies--;
    }

    void FinishWaves()
    {
        Powerup[] loadedPowerups = Resources.LoadAll<Powerup>("Prefabs/Powerups");
        allPowerups.AddRange(loadedPowerups);
        Debug.Log("enter a message here");
        UIManager.Instance.ShowMessage("message here");
        if (animator == null)
            Debug.Log("no animator found");
        else {
            crystalLight.intensity = 1.5f;
            animator.enabled = true;
        }
    }
}

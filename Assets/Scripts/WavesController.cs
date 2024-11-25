using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class WavesController : MonoBehaviour
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

    public Sprite defenseCompleteSprite;

    public GameObject abilityChestToSpawn;
    public GameObject StoryChestSpawnToSpawn;

    private List<Transform> spawnPoints;
    private int currentWaveIndex = 0;
    private int nbEnemies = 0;
    private float waveTimer;
    private bool wavesEnded = false;
    private bool startedSpawning = false;
    private bool isSpawning = false;
    private bool waveEarilyShutdown = false;
    private Transform enemiesTarget;

    [Header("Cristal Configuration")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public Light2D crystalLight;
    private Sprite defaultSprite;
    private float lightIntensity;

    private List<GameObject> spawnedEnemyList = new List<GameObject>();


    public GameObject HealthBar;
    private Image healthImage;
    public Color fullHealthColor = Color.cyan;
    public Color lowHealthColor = Color.red;


    [SerializeField] public float towerHealth; // Tower's health
    private float towerFullHealth;

    void Start()
    {
        towerFullHealth = towerHealth;
        GameObject healthBarPrefab = Resources.Load<GameObject>("Prefabs/UI/TowerHealthBar");
        if (healthBarPrefab != null)
        {
            HealthBar = Instantiate(healthBarPrefab, transform.position, Quaternion.identity, transform);
            Transform amountDisplayTransform = HealthBar.transform.Find("Frame").Find("AmountDisplay");
            if (amountDisplayTransform != null)
            {
                healthImage = amountDisplayTransform.GetComponent<Image>();
            }
            HealthBar.SetActive(false);
        }
    }

    public void UpdateTowerHealth(float percentage)
    {
        HealthBar.SetActive(true);
        if (healthImage != null)
        {
            healthImage.fillAmount = percentage;
            healthImage.color = Color.Lerp(lowHealthColor, fullHealthColor, percentage);
        }
    }

    public void TowerTakeDamage(float damage)
    {
        towerHealth -= damage;
        Debug.Log("Tower Health: " + towerHealth);

        UpdateTowerHealth(towerHealth/towerFullHealth);
        if (towerHealth <= 0)
        {
            //EndGame();
            GameController.Instance.DefenseFailed();

        }
    }

    public void StartDefense()
    {
        towerHealth *= (towerHealth * GameController.Instance.TowerDefensed+1);
        towerFullHealth = towerHealth;
        InitSpawPoints();
        crystalLight.pointLightOuterRadius = 8f;
        crystalLight.intensity = 0.55f;
        crystalLight.pointLightOuterRadius = 2.25f;

        startedSpawning = true;


        defaultSprite = spriteRenderer.sprite;
        lightIntensity = crystalLight.intensity;

        spriteRenderer.sprite = defaultSprite;
        crystalLight.intensity = 0.5f;
        animator.enabled = false;

        if (waves.Count > 0)
        {
            StartCoroutine(StartNextWave());
        }


        GameController.Instance.StartDefenceAWave(this);
    }









    void InitSpawPoints()
    {
        float searchRadius = 10f;
        spawnPoints = new List<Transform>();
        GameObject[] spawnPointObjects = GameObject.FindGameObjectsWithTag("EnemieSpawnPoint");

        foreach (GameObject obj in spawnPointObjects)
        {
            float distance = Vector3.Distance(transform.position, obj.transform.position);

            // add all the spawnpoints that is within the range to the list.
            if (distance <= searchRadius)
            {
                spawnPoints.Add(obj.transform);
            }
        }
        enemiesTarget = transform;

        Debug.Log("A total of " + spawnPoints.Count + " Spawn point for Crystal has been loaded!");

    }

    void Update()
    {
        if (!isSpawning && startedSpawning)
        {
            if (waveTimer <= 0)
            {
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


        waveTimer -= Time.deltaTime;
        if (waveTimer < 0 && startedSpawning && !waveEarilyShutdown)
        {
            waveEarilyShutdown = true;
        }

    }

    IEnumerator StartNextWave()
    {

        UIManager.Instance.ShowDefenseWaveRemain(currentWaveIndex + 1, waves.Count, waves[currentWaveIndex].waveDuration);
        if (animator == null)
            Debug.Log("no animator found");
        else
        {
            EffectsManager.Instance.PlaySFX(19,0.3f);
            crystalLight.intensity = 1.0f;
            animator.enabled = true;
        }
        isSpawning = true;
        wavesEnded = false;
        Wave currentWave = waves[currentWaveIndex];
        nbEnemies = currentWave.baseEnemyCount;
        waveTimer = currentWave.waveDuration;
        waveEarilyShutdown = false;
        Debug.Log("Current Wave duration: " + waveTimer);

        while (nbEnemies > 0 && isSpawning && !waveEarilyShutdown)
        {
            SpawnEnemy(currentWave);
            yield return new WaitForSeconds(currentWave.spawnInterval);
        }

        if (animator == null)
            Debug.Log("no animator found");
        else{
            animator.enabled = false;
            spriteRenderer.sprite = defaultSprite;
            crystalLight.intensity = 0.55f;
        }

        yield return new WaitForSeconds(timeBetweenWaves);
        wavesEnded = true;
        isSpawning = false;
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

        GameObject enemy = EnemyManager.Instance.SummonEenemy(selectedEnemy.enemyPrefab, 
            spawnPoint, GameController.Instance.CurrentDifficulty, transform, false);

        spawnedEnemyList.Add(enemy);


        //GameObject enemy = Instantiate(selectedEnemy.enemyPrefab, spawnPoint.position, Quaternion.identity);
        enemy.transform.parent = transform;
        enemy.SetActive(true);

        //SampleEnemy enemyComponent = enemy.GetComponentInChildren<SampleEnemy>();
        //if (enemyComponent != null)
        //{

        //    //enemyComponent.Initialize(enemiesTarget);
        //}
        //else
        //{
        //    Debug.Log("enemyComponent not found");
        //}
        nbEnemies--;
    }

    public void DestroyAllEnemies()
    {
        for (int i = spawnedEnemyList.Count - 1; i >= 0; i--)
        {
            GameObject enemy = spawnedEnemyList[i];

            if (enemy != null)
            {
                SampleEnemy sampleEnemy = enemy.GetComponent<SampleEnemy>();
                if (sampleEnemy != null)
                {
                    // 调用SampleEnemy的DeathSlient()方法
                    sampleEnemy.DeathSlient();
                }
                else
                {
                    Destroy(enemy);
                }
            }

            //destory all spawned enemy after the defense is finished.
            spawnedEnemyList.RemoveAt(i);
        }
    }

    void FinishWaves()
    {
        //Powerup[] loadedPowerups = Resources.LoadAll<Powerup>("Prefabs/Powerups");
        //allPowerups.AddRange(loadedPowerups);
        
        UIManager.Instance.ShowMessage("Defense Complete!");

        DestroyAllEnemies();
        startedSpawning = false;
        GameController.Instance.PlayerFinishedDefense();

        EffectsManager.Instance.PlaySFX(20);
        if (animator == null)
            Debug.Log("no animator found");
        else
        {

            crystalLight.intensity = 0.8f;
            crystalLight.pointLightOuterRadius = 10f;
            crystalLight.pointLightOuterRadius = 3f;
        }



        Invoke("DefenseCompleteReward", 2.2f);

    }


    void DefenseCompleteReward()
    {

        EffectsManager.Instance.PlaySFX(16);
        if (animator == null)
            Debug.Log("no animator found");
        else
        {

            crystalLight.intensity = 1.5f;
            animator.enabled = false;
            spriteRenderer.sprite = defenseCompleteSprite;
            crystalLight.pointLightOuterRadius = 15f;
            crystalLight.pointLightOuterRadius = 6f;
        }

        Invoke("DefenseCompleteSpawningChest", 0.95f);


    }


    void DefenseCompleteSpawningChest()
    {
        EffectsManager.Instance.PlaySFX(21);
        Instantiate(abilityChestToSpawn, new Vector3(transform.position.x - 2, transform.position.y, transform.position.z), Quaternion.identity);
        Instantiate(abilityChestToSpawn, new Vector3(transform.position.x + 2, transform.position.y, transform.position.z), Quaternion.identity);

        Instantiate(StoryChestSpawnToSpawn, new Vector3(transform.position.x, transform.position.y - 1.9f, transform.position.z), Quaternion.identity);
    }
}

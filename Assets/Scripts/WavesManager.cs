using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavesManager : MonoBehaviour
{
    public GameObject ennemyPrefab;
    public List<GameObject> spawnPoints;
    public int nb_ennemies_at_start = 2;
    public int nb_waves = 5;
    public float waves_duration = 5.0f;

    private int nb_ennemies;
    private float timer;

    void Start()
    {
        nb_ennemies = nb_ennemies_at_start;
    }

    void UpdateTimer()
    {
        timer -= Time.deltaTime;

        if (timer <= 0.0f)
        {
            UpdateWave();
        }
    }

    float ResetTimer()
    {
        timer = waves_duration;
        return timer;
    }

    int UpdateNbEnnemies()
    {
        nb_ennemies = (int)((float)nb_ennemies * 1.5f);
        return nb_ennemies;
    }

    void UpdateWave()
    {
        if (nb_waves > 0)
        {
            nb_waves--;
            CreateWave(UpdateNbEnnemies(), ResetTimer());
        }
        else
        {
            FinishTowerDefense();
        }
    }

    void CreateWave(int nb_ennemies, float timer)
    {
        foreach (GameObject spawn_point in spawnPoints)
        {
            for (int i = 0; i < nb_ennemies / spawnPoints.Count; ++i)
            {
                GameObject ennemy = Instantiate(ennemyPrefab, spawn_point.transform.position, Quaternion.identity);
                ennemy.transform.parent = transform;
                ennemy.SetActive(true);
            }
        }
        for(int i = 0; i < nb_ennemies % spawnPoints.Count; ++i)
        {
            GameObject ennemy = Instantiate(ennemyPrefab, spawnPoints[Random.Range(0, spawnPoints.Count)].transform.position, Quaternion.identity);
            ennemy.transform.parent = transform;
            ennemy.SetActive(true);
        }
        ResetTimer();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTimer();
    }

    void FinishTowerDefense()
    {
        Debug.Log("Waves end");
    }
}

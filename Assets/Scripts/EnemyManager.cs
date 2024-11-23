using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] public GameObject [] Enemies;

    public GameObject spawn;
    public GameObject [] crystals;
    Transform Player;
    Transform closestCrystal;
    float [] distanceToCrystals;


    void Start(){
        // distanceToCrystals = new float[crystals.Length];
        Player = GameObject.Find("Player").transform.GetChild(0);
        // spawnEnemy(spawn, 1f);
    }

    public Transform getClosestTower(Transform enemy){
        float smallestDistance = Mathf.Infinity;
        Transform closestCrystal = enemy;
        for (int n = 0; n < crystals.Length; n++){
            distanceToCrystals[n] = Vector2.Distance(enemy.position, crystals[n].transform.position);
        }
        for (int n = 0; n < distanceToCrystals.Length; n++){
            if (distanceToCrystals[n] < smallestDistance)
                smallestDistance = distanceToCrystals[n];
                closestCrystal = crystals[n].transform;
        }
        return closestCrystal;
    }

    public GameObject GetRandomEnemy(){
        //Random.InitState((int) System.DateTime.Now.Ticks);
        int index = Random.Range(0, Enemies.Length);
        Debug.Log("INDEX: " + index);
        Debug.Log("LENGTH: " + Enemies.Length);

        if (Enemies[index] != null){
            return Enemies[index];
        }
        return null;
    }

    void spawnEnemy(GameObject SpawnPoint, float difficultyLevel){
        GameObject prefab = GetRandomEnemy();
        prefab = Instantiate(prefab, SpawnPoint.transform.position, Quaternion.identity);

        prefab = prefab.transform.GetChild(0).gameObject;
        prefab.GetComponent<SampleEnemy>().SetDifficulty(difficultyLevel);
        prefab.GetComponent<SampleEnemy>().SetTarget(Player);


        // Check if any child GameObject has a component that is a subclass of ParentClass
        SampleEnemy foundComponent = prefab.GetComponentInChildren<SampleEnemy>();

        if (foundComponent is TowerEnemy){
            closestCrystal = getClosestTower(prefab.transform);
            foundComponent.SetTower(closestCrystal);
            foundComponent.SetPlayer(Player);
        }


        // Could add something like below to add probabilities for which enemy spawns
        // float rand = Random.Range(0,1);

        // // 40% chance for enemy 1
        // if (rand < 0.4f){
        //     prefab = Instantiate(Enemies[0], SpawnPoint.transform.position, Quaternion.identity);
        // }
        // // 20 % chance for some other enemy
        // if (rand < 0.6f){
        //     // Spawn this enemy instead
        // }
        // // 40% chance for HeavyTowerEnemy
        // else{
        //     prefab = Instantiate(Enemies[0], SpawnPoint.transform.position, Quaternion.identity);
        // }
        // prefab.transform.GetChild(0).GetComponent<SampleEnemy>().SetDifficulty(difficultyLevel);

    }



}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] public GameObject [] Enemies;
    public static EnemyManager Instance { get; private set; }

    Transform Player;


    void Start(){

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // distanceToCrystals = new float[crystals.Length];
        Player = GameObject.Find("Player").transform.GetChild(0);
        // spawnEnemy(spawn, 1f);
    }

    public GameObject GetRandomEnemy(){
        int index = Random.Range(0, Enemies.Length);
        return Enemies[index];
    }


    public GameObject SummonEenemy(GameObject inputEnemy, Transform tf, float difficultyLevel)
    {
        return SummonEenemy(inputEnemy, tf, difficultyLevel, null);
    }



    public GameObject SummonEenemy(GameObject inputEnemy, Transform tf, float difficultyLevel, Transform TowerTF)
    {


        GameObject enemyObj = Instantiate(inputEnemy, tf.position, Quaternion.identity);

        enemyObj.GetComponent<SampleEnemy>().SetDifficulty(difficultyLevel);
        enemyObj.GetComponent<SampleEnemy>().SetTarget(Player);


        // Check if any child GameObject has a component that is a subclass of ParentClass
        SampleEnemy foundComponent = enemyObj.GetComponentInChildren<SampleEnemy>();

        if (foundComponent is TowerEnemy)
        {
            if (TowerTF != null)
            {
                foundComponent.SetTower(TowerTF);
            }
            foundComponent.SetPlayer(Player);
        }
        return enemyObj;

    }
    public GameObject SummonEenemy(GameObject inputEnemy, Transform tf, float difficultyLevel, Transform TowerTF,bool isNormalSpawned)
    {


        GameObject enemyObj = Instantiate(inputEnemy, tf.position, Quaternion.identity);

        enemyObj.GetComponent<SampleEnemy>().SetDifficulty(difficultyLevel);
        enemyObj.GetComponent<SampleEnemy>().SetTarget(Player);


        // Check if any child GameObject has a component that is a subclass of ParentClass
        SampleEnemy foundComponent = enemyObj.GetComponentInChildren<SampleEnemy>();
        foundComponent.IsNormalSpawned = isNormalSpawned;

        if (foundComponent is TowerEnemy)
        {
            if (TowerTF != null)
            {
                foundComponent.SetTower(TowerTF);
            }
            foundComponent.SetPlayer(Player);
        }
        return enemyObj;

    }





}

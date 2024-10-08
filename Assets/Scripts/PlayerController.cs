using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [SerializeField] public float playerHealth = 20;


    void Start()
    {
        if (Instance == null)
        {
            Instance = this;  
            DontDestroyOnLoad(gameObject);  
        }
        else
        {
            Destroy(gameObject);  
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void takesDamage(float damage)
    {
        playerHealth -= damage;
        if (playerHealth <= 0)
        {
            playerHealth = 0;
            Debug.Log("Player Is Dead!");
        }

        Debug.Log("Player takes damage: the new health is " + playerHealth );

    }
}

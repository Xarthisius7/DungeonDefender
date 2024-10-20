using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }
    // public PlayerController Instance { get; private set; }

    public float playerHealth;

    public GameOverScript GameOver;


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
            GameOver.Setup("You Died");

            // GameOverScript.Instance.Setup("You Died");
        }

        Debug.Log("Player takes damage: the new health is " + playerHealth );

    }
}

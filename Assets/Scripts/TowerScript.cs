using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerScript : MonoBehaviour
{
    // TO DO: Make a script so that when an enemy touches the tower it takes damage
    // and if it takes enough damage the game ends

    [SerializeField] public float towerHealth; // Tower's health
    [SerializeField] public GameController controller;
    [SerializeField] private Transform player;
    //[SerializeField] private GameObject InteractKey;

    bool isActive = false;
    float distanceToPlayer;

    void Update(){
        if(!isActive){
            // Display the 'E' key when the user gets close to tower
            // To indicate to the useer that they can activate it
            DisplayKey();
        }
        
        if (isActive){
            // Tower was activated so start waves
            Debug.Log("Crystal was activated");
        }
    }

    void DisplayKey(){
        distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= 2){
            //InteractKey.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
                isActive = true;  
        }
        //else
            //InteractKey.SetActive(false);
    }

    // Function to take damage from the enemy
    public void TakeDamage(float damage)
    {
        towerHealth -= damage;
        Debug.Log("Tower Health: " + towerHealth);

        if (towerHealth <= 0)
        {
            //EndGame();
            controller.GameOver();

        }
    }
}
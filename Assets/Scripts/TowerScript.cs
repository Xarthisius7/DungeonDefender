using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerScript : MonoBehaviour
{
    // TO DO: Make a script so that when an enemy touches the tower it takes damage
    // and if it takes enough damage the game ends

    [SerializeField] public float towerHealth; // Tower's health
    // [SerializeField] public GameOverScript GameOverScreen;
    [SerializeField] public GameController controller;

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
    // End the game when the tower's health reaches 0
    // void EndGame()
    // {
    //     Debug.Log("Game Over! The tower has been destroyed.");
    //     GameOver.Setup("Tower Destroyed");
    // }
}
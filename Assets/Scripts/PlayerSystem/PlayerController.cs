using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }
    // public PlayerController Instance { get; private set; }


    [SerializeField] public float defaultPlayerMaxHealth = 20;
    [SerializeField] public float defaultPlayerMaxStamina = 100;
    [SerializeField] public float defaultPlayerAttack = 5; // the damage dualt by player.

    [SerializeField] public float defaultStaminaRegenRate = 10f;  // how much stamina regen per second by default
    [SerializeField] private float defaultDashStaminaCost = 40f; // default cost of a dash
    [SerializeField] public float shootingStaminaDelay = 0.5f; // how long the stamina will start to regenerate after shooting.


    [SerializeField] public float defaultPlayerShootSpeed = 0.7f;

    [SerializeField] public GameController controller;


    private float currentPMaxHealth;
    private float currentPMaxStamina;

    private float currentPlayerHealth;
    private float currentPlayerStamina;
    private float currentPlayerAttack;

    private float currentStaminaRegenRate;
    private float currentDashStaminaCost;

    private float currentPlayerShootSpeed;


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
        currentPMaxHealth = defaultPlayerMaxHealth;
        currentPMaxStamina = defaultPlayerMaxStamina;

        currentPlayerHealth = defaultPlayerMaxHealth;
        currentPlayerStamina = defaultPlayerMaxStamina;
        currentPlayerAttack = defaultPlayerAttack;

        currentStaminaRegenRate = defaultStaminaRegenRate;
        currentDashStaminaCost = defaultDashStaminaCost;

        currentPlayerShootSpeed = defaultPlayerShootSpeed;

    }

    void Update()
    {


    }

    public bool TryUseStamiaToDash()
    {
        if(currentPlayerStamina > currentDashStaminaCost)
        {
            // if player have enough stamina, allows to dash & consume the stamina
            UpdateCurrentPlayerStamina(-currentDashStaminaCost);
            return true;
        }
        return false;
    }

    public void RegenTimeFixedStamina()
    {
        if (currentPlayerStamina < currentPMaxStamina)
        {
            currentPlayerStamina += currentStaminaRegenRate * Time.deltaTime;
            currentPlayerStamina = Mathf.Clamp(currentPlayerStamina, 0, currentPMaxStamina);
            UIManager.Instance.UpdateStamina(currentPlayerStamina / currentPMaxStamina);
        }
    }


    public void UpdatePlayerHealth(float change)
    {
        // changing player's health - can be triggerd by events, healing, etc.
        currentPlayerHealth += change;
        checkDeath();
        UIManager.Instance.UpdateHealth(currentPlayerHealth / currentPMaxHealth);

        Debug.Log("Player health changed: the new health is " + currentPlayerHealth);

    }

    public void PlayerTakesDamage(float damage)
    {
        // player taking damage from enemy - can trigger powerup events in future.
        currentPlayerHealth -= damage;
        checkDeath();
        UIManager.Instance.UpdateHealth(currentPlayerHealth / currentPMaxHealth);

        UIManager.Instance.UpdateHealth(currentPlayerHealth / currentPMaxHealth);
        Debug.Log("Player takes damage: the new health is " + currentPlayerHealth);

    }

    private void checkDeath()
    {
        if (currentPlayerHealth <= 0)
        {
            currentPlayerHealth = 0;
            Debug.Log("Player Is Dead!");

            controller.GameOver();

            // GameOver.Setup("You Died");
            // GameOverScript.Instance.Setup("You Died");
        }

    }

    // Get 
    public float GetCurrentPlayerHealth()
    {
        return currentPlayerHealth;
    }

    public float GetCurrentPlayerStamina()
    {
        return currentPlayerStamina;
    }

    public float GetCurrentPlayerAttack()
    {
        return currentPlayerAttack;
    }

    public float GetCurrentStaminaRegenRate()
    {
        return currentStaminaRegenRate;
    }

    public float GetCurrentDashStaminaCost()
    {
        return currentDashStaminaCost;
    }

    public float GetCurrentPMaxHealth()
    {
        return currentPMaxHealth;
    }

    public float GetCurrentPMaxStamina()
    {
        return currentPMaxStamina;
    }

    public float GetCurrentPlayerShootSpeed()
    {
        return currentPlayerShootSpeed;
    }

    // Set 
    public void SetCurrentPlayerHealth(float value)
    {
        currentPlayerHealth = Mathf.Max(0, value);
        UIManager.Instance.UpdateHealth(currentPlayerHealth / currentPMaxHealth);
    }

    public void SetCurrentPlayerStamina(float value)
    {
        currentPlayerStamina = Mathf.Max(0, value);
        UIManager.Instance.UpdateStamina(currentPlayerStamina / currentPMaxStamina);
    }

    public void SetCurrentPlayerAttack(float value)
    {
        currentPlayerAttack = Mathf.Max(0, value);
    }

    public void SetCurrentStaminaRegenRate(float value)
    {
        currentStaminaRegenRate = Mathf.Max(0, value);
    }

    public void SetCurrentDashStaminaCost(float value)
    {
        currentDashStaminaCost = Mathf.Max(0, value);
    }

    public void SetCurrentPlayerShootSpeed(float value)
    {
        currentPlayerShootSpeed = Mathf.Max(0, value); 
    }

    // Update 
    public void UpdateCurrentPlayerHealth(float delta)
    {
        currentPlayerHealth = Mathf.Max(0, currentPlayerHealth + delta);
        UIManager.Instance.UpdateHealth(currentPlayerHealth / currentPMaxHealth);
    }

    public void UpdateCurrentPlayerStamina(float delta)
    {
        currentPlayerStamina = Mathf.Max(0, currentPlayerStamina + delta);
        UIManager.Instance.UpdateStamina(currentPlayerStamina/currentPMaxStamina);
    }

    public void UpdateCurrentPlayerAttack(float delta)
    {
        currentPlayerAttack = Mathf.Max(0, currentPlayerAttack + delta);
    }

    public void UpdateCurrentStaminaRegenRate(float delta)
    {
        currentStaminaRegenRate = Mathf.Max(0, currentStaminaRegenRate + delta);
    }

    public void UpdateCurrentDashStaminaCost(float delta)
    {
        currentDashStaminaCost = Mathf.Max(0, currentDashStaminaCost + delta);
    }

    public void SetCurrentPMaxHealth(float value)
    {
        currentPMaxHealth = Mathf.Max(0, value); 
    }

    public void SetCurrentPMaxStamina(float value)
    {
        currentPMaxStamina = Mathf.Max(0, value);
    }
    public void UpdateCurrentPlayerShootSpeed(float delta)
    {
        currentPlayerShootSpeed = Mathf.Max(0, currentPlayerShootSpeed + delta); 
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }
    // public PlayerController Instance { get; private set; }


    [SerializeField] private float defaultDashStaminaCost = 40f; // default cost of a dash
    [SerializeField] public float shootingStaminaDelay = 0.5f; // how long the stamina will start to regenerate after shooting.


    [SerializeField] public GameController controller;


    private float currentPlayerHealth;
    private float currentPlayerStamina;
    private float currentPlayerAttack;

    private float currentDashStaminaCost;

    private bool secondChanceTriggered = false;

    private float selfHealTimer = 0f; 


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

        Invoke("DelayInitStats", 0.1f);

        currentDashStaminaCost = defaultDashStaminaCost;


    }
    private void DelayInitStats()
    {
        currentPlayerHealth = PowerupManager.instance.GetAttributeValue("MaxHealth"); ;
        currentPlayerStamina = PowerupManager.instance.GetAttributeValue("MaxStamina"); ;
        currentPlayerAttack = PowerupManager.instance.GetAttributeValue("Attack");
    }

    void Update()
    {
        selfHealTimer += Time.deltaTime; 

        if (selfHealTimer >= 1f) 
        {
            selfHealTimer = 0f;

            if (PowerupManager.instance.IsMagicActive(10))
            {
                PlayerTakesPercentDamage(-0.02f);
            }
        }

    }

    public bool TryUseStamiaToDash()
    {
        if(currentPlayerStamina > currentDashStaminaCost)
        {
            // if player have enough stamina, allows to dash & consume the stamina

            if (PowerupManager.instance.IsMagicActive(7))
            {

                UpdateCurrentPlayerStamina(-(currentDashStaminaCost/2));
            }
            else
            {
                UpdateCurrentPlayerStamina(-currentDashStaminaCost);
            }
            return true;
        }
        return false;
    }

    public void RegenTimeFixedStamina()
    {
        if (currentPlayerStamina < PowerupManager.instance.GetAttributeValue("MaxStamina"))
        {
            currentPlayerStamina += PowerupManager.instance.GetAttributeValue("StaminaRegenSpeed") * Time.deltaTime;
            currentPlayerStamina = Mathf.Clamp(currentPlayerStamina, 0, PowerupManager.instance.GetAttributeValue("MaxStamina"));
            UIManager.Instance.UpdateStamina(currentPlayerStamina / PowerupManager.instance.GetAttributeValue("MaxStamina"));
        }
    }


    public void UpdatePlayerHealth(float change)
    {
        // changing player's health - can be triggerd by events, healing, etc.
        currentPlayerHealth += change;
        if(currentPlayerHealth> PowerupManager.instance.GetAttributeValue("MaxHealth"))
        {
            currentPlayerHealth = PowerupManager.instance.GetAttributeValue("MaxHealth");
        }
        checkDeath();
        UIManager.Instance.UpdateHealth(currentPlayerHealth / PowerupManager.instance.GetAttributeValue("MaxHealth"));


    }

    public void PlayerTakesDamage(float damage)
    {
        if (PowerupManager.instance.IsMagicActive(20))
        {
            float randomValue = Random.Range(0f, 100f);
            if (randomValue < 50f)
            {

                EffectsManager.Instance.PlaySFX(38, 0.8f);
                return;
            }
        }

        float defense = PowerupManager.instance.GetAttributeValue("Defense");
        float damageReductionRate = defense / (defense + 50);
        float actualDamage = damage * (1 - damageReductionRate);
        currentPlayerHealth -= actualDamage;

        EffectsManager.Instance.PlaySFX(36,0.8f);

        checkDeath();
        UIManager.Instance.UpdateHealth(currentPlayerHealth / PowerupManager.instance.GetAttributeValue("MaxHealth"));


    }


    public void PlayerTakesPercentDamage(float Percent)
    {
        // player taking damage from enemy - can trigger powerup events in future.
        currentPlayerHealth -= PowerupManager.instance.GetAttributeValue("MaxHealth")*Percent;
        if (currentPlayerHealth > PowerupManager.instance.GetAttributeValue("MaxHealth"))
        {
            currentPlayerHealth = PowerupManager.instance.GetAttributeValue("MaxHealth");
        }
        checkDeath();
        UIManager.Instance.UpdateHealth(currentPlayerHealth / PowerupManager.instance.GetAttributeValue("MaxHealth"));


    }


    private void checkDeath()
    {
        if (currentPlayerHealth <= 0)
        {
            currentPlayerHealth = 0;
            Debug.Log("Player Is Dead!");
            EffectsManager.Instance.PlaySFX(35, 1.0f);

            if (!secondChanceTriggered && PowerupManager.instance.IsMagicActive(19))
            {
                //revive player from powerup
                secondChanceTriggered = true;
                currentPlayerHealth = 0.5f* PowerupManager.instance.GetAttributeValue("MaxHealth");

                UIManager.Instance.UpdateHealth(currentPlayerHealth / PowerupManager.instance.GetAttributeValue("MaxHealth"));
                UIManager.Instance.BroadcastMessage("Second chance revived you from death... ");
                EffectsManager.Instance.PlaySFX(6, 1.0f);
            }
            else
            {

                controller.GameOver();
            }


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

    public float GetCurrentDashStaminaCost()
    {
        return currentDashStaminaCost;
    }

    // Set 
    public void SetCurrentPlayerHealth(float value)
    {
        currentPlayerHealth = Mathf.Max(0, value);
        UIManager.Instance.UpdateHealth(currentPlayerHealth / PowerupManager.instance.GetAttributeValue("MaxHealth"));
    }

    public void SetCurrentPlayerStamina(float value)
    {
        currentPlayerStamina = Mathf.Max(0, value);
        UIManager.Instance.UpdateStamina(currentPlayerStamina / PowerupManager.instance.GetAttributeValue("MaxStamina"));
    }

    public void SetCurrentDashStaminaCost(float value)
    {
        currentDashStaminaCost = Mathf.Max(0, value);
    }

    // Update 
    public void UpdateCurrentPlayerHealth(float delta)
    {
        currentPlayerHealth = Mathf.Max(0, currentPlayerHealth + delta);
        UIManager.Instance.UpdateHealth(currentPlayerHealth / PowerupManager.instance.GetAttributeValue("MaxHealth"));
    }

    public void UpdateCurrentPlayerStamina(float delta)
    {
        currentPlayerStamina = Mathf.Max(0, currentPlayerStamina + delta);
        UIManager.Instance.UpdateStamina(currentPlayerStamina/ PowerupManager.instance.GetAttributeValue("MaxStamina"));
    }


    public void UpdateCurrentDashStaminaCost(float delta)
    {
        currentDashStaminaCost = Mathf.Max(0, currentDashStaminaCost + delta);
    }

}

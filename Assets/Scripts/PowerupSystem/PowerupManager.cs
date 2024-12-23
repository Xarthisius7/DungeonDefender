using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public class AttributeBoost
{
    public string attributeName; // Attribute name
    public string boostValue;    // Boost value 
}

public class PowerupManager : MonoBehaviour
{
    public static PowerupManager instance;

    public Dictionary<string, float> baseAttributes = new Dictionary<string, float>()
    {
        { "Attack", 10 },
        { "AttackSpeed", 1 },
        { "Speed", 1.1f },
        { "Defense", 5 },
        { "MaxHealth", 100 },
        { "MaxStamina", 100 },
        { "StaminaRegenSpeed", 15 }
    };

    private Dictionary<string, float> currentAttributes;
    private List<Powerup> allPowerups = new List<Powerup>();
    public List<Powerup> ownedPowerups = new List<Powerup>();

    public Light2D crystalLight;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            InitializeAttributes();
            LoadAllPowerups();

            
        }
        else
        {
            Destroy(gameObject);
        }




    }
    




    // Load all Powerups from the Resources folder
    public void LoadAllPowerups()
    {
        Powerup[] loadedPowerups = Resources.LoadAll<Powerup>("Prefabs/Powerups");
        allPowerups.AddRange(loadedPowerups);
        Debug.Log("A total of " + loadedPowerups.Length + " Powerup has been loaded. ");
    }

    // Reset all powerups and attributes to their base values
    public void ResetAllPowerups()
    {
        InitializeAttributes();
        ownedPowerups.Clear();
        UpdatePowerups();
    }

    // Initialize base attributes and reset current attribute values
    private void InitializeAttributes()
    {
        currentAttributes = new Dictionary<string, float>(baseAttributes);
    }

    // Add a Powerup to the player by ID
    public void GivePowerup(int id)
    {
        Powerup powerup = LoadPowerup(id);
        if (powerup != null)
        {
            if (powerup.isMagic)
            {
                if (!ownedPowerups.Exists(p => p.isMagic && p.id == id))
                {
                    ownedPowerups.Add(powerup);
                }
            }
            else
            {
                ownedPowerups.Add(powerup);
            }
            UIManager.Instance.ShowMessage("} - You obtained a Powerup: " + powerup.name+" - {");

            UpdatePowerups();
        }
    }

    // Remove a random Powerup from the player's owned list
    public void RemoveRandomPowerups()
    {
        if (ownedPowerups.Count > 0)
        {
            int index = Random.Range(0, ownedPowerups.Count);
            ownedPowerups.RemoveAt(index);
            UpdatePowerups();
        }
    }

    // Load a random Powerup that is not already owned if it's a Magic type
    public Powerup LoadRandomPowerups()
    {
        List<Powerup> availablePowerups = allPowerups.FindAll(p => !(p.isMagic && ownedPowerups.Exists(op => op.id == p.id)));
        if (availablePowerups.Count > 0)
        {
            return availablePowerups[Random.Range(0, availablePowerups.Count)];
        }
        return null;
    }

    // Load a Powerup by ID
    public Powerup LoadPowerup(int id)
    {
        Powerup powerup = allPowerups.Find(p => p.id == id);
        if (powerup == null)
        {
            Debug.LogError($"Powerup with ID {id} does not exist. Returning a random Powerup.");
            return LoadRandomPowerups();
        }
        return powerup;
    }

    // Update the player's attributes based on owned Powerups
    public void UpdatePowerups()
    {
        InitializeAttributes();
        Dictionary<string, float> additiveBoosts = new Dictionary<string, float>();
        Dictionary<string, float> multiplicativeBoosts = new Dictionary<string, float>();

        // Initialize additive and multiplicative boosts
        foreach (string attribute in baseAttributes.Keys)
        {
            additiveBoosts[attribute] = 0f;
            multiplicativeBoosts[attribute] = 0f;
        }

        // Calculate total additive and multiplicative boosts for each attribute
        foreach (Powerup powerup in ownedPowerups)
        {
            foreach (AttributeBoost boost in powerup.boosts)
            {
                float boostValue;
                if (boost.boostValue.Contains("%"))
                {
                    boostValue = float.Parse(boost.boostValue.Replace("%", "")) / 100f;
                    multiplicativeBoosts[boost.attributeName] += boostValue;
                }
                else
                {
                    boostValue = float.Parse(boost.boostValue);
                    additiveBoosts[boost.attributeName] += boostValue;
                }
            }
        }

        UpdateSpellPowerupSpecialEffects();

        // Update current attributes based on additive and multiplicative boosts
        foreach (string attribute in baseAttributes.Keys)
        {
            currentAttributes[attribute] = (baseAttributes[attribute] + additiveBoosts[attribute]) * (1 + multiplicativeBoosts[attribute]);
        }


    }

    private void UpdateSpellPowerupSpecialEffects()
    {
        if (IsMagicActive(6))
        {
            ChangeLightRadius();
        }

    }


    public void ChangeLightRadius()
    {

        crystalLight.pointLightOuterRadius = 12f;
        crystalLight.pointLightInnerRadius = 4f;
        crystalLight.intensity = 1f;
    }
    




    // Check if a specific Magic Powerup is active
    public bool IsMagicActive(int id)
    {
        return ownedPowerups.Exists(p => p.isMagic && p.id == id);
    }

    // Get the current value of a specific attribute
    public float GetAttributeValue(string attributeName)
    {
        if (currentAttributes.ContainsKey(attributeName))
        {
            return currentAttributes[attributeName];
        }
        Debug.LogError($"Attribute {attributeName} does not exist.");
        return 0;
    }
}

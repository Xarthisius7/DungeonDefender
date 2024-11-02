using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Consumable item, stackable, consumable upon use, e.g. potions
[CreateAssetMenu(fileName = "ConsumableItem", menuName = "ScriptableObjects/ConsumableItem")]
public class ConsumableItem : ItemScriptableObject
{
    

    private void OnEnable()
    {
        consumable = true; // Set consumable to true
        maxStack = 10; // Default stack limit
    }

    public override bool Use()
    {
        Debug.Log($"Using {itemName}...");
        return true; // Consumable upon use
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Attribute boost item, non-consumable, non-stackable, Generally can be equiped. (weapons with stats)
[CreateAssetMenu(fileName = "AttributeBoostItem", menuName = "ScriptableObjects/AttributeBoostItem")]
public class AttributeBoostItem : ItemScriptableObject
{
    [SerializeField]
    public List<AttributeBoost> attributeBoosts = new List<AttributeBoost>(); // Attribute boost list

    private void OnEnable()
    {
        consumable = false; // Set consumable to false
        maxStack = 1; // Set max stack to 1
    }

    public override bool Use()
    {
        Debug.Log("This item provides attribute boosts and cannot be consumed.");
        return false; // Cannot be used
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConsumableItemData", menuName = "ScriptableObjects/ConsumableItemData")]
public class ConsumableItemScriptableObj : ItemScriptableObject
{
    //For Consumable
    public int buffPower;
    public BuffType buffType;
    public float buffTime;
    public bool permanent;

    public override bool use()
    {
        Debug.Log("Use Consumable");
        return true;
    }

    public enum BuffType
    {
        HP, ATTACK_POW, ATTACK_SPEED, DEFENSE, SPEED
    }
}

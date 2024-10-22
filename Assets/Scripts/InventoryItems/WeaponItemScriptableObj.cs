using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponItemData", menuName = "ScriptableObjects/WeaponItemData")]
public class WeaponItemScriptableObj : ItemScriptableObject
{
    public Sprite bulletSprite;
    public int attackPower;
    public int attackSpeed;

    private void OnEnable()
    {
        maxStack = 1;
    }

    //Using a weapon will equip it
    public override bool use()
    {
        Debug.Log("Equip Weapon");
        return true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponItemData", menuName = "ScriptableObjects/WeaponItemData")]
public class WeaponItemScriptableObj : ItemScriptableObject
{
    public Sprite bulletSprite;
    public int attackPower;
    public int attackSpeed;

    public override bool use()
    {
        throw new System.NotImplementedException();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TrapItemData", menuName = "ScriptableObjects/TrapItemData")]
public class TrapItemScriptableObj : ItemScriptableObject
{
    public GameObject trapPrefab;
    public int attackPower;
    public int attackSpeed;
    public float slowdown;

    public override bool use()
    {
        throw new System.NotImplementedException();
    }
}

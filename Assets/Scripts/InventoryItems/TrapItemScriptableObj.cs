using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TrapItemData", menuName = "ScriptableObjects/TrapItemData")]
public class TrapItemScriptableObj : ItemScriptableObject
{
    public GameObject trapPrefab;

    public override bool use()
    {
        throw new System.NotImplementedException();
    }
}

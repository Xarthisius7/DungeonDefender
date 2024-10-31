using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BookItemData", menuName = "ScriptableObjects/BookItemData")]
public class BookItemScriptableObj : ItemScriptableObject
{
    public string title;
    public string description;

    public override bool use()
    {
        Debug.Log("Open Book");
        return true;
    }
}

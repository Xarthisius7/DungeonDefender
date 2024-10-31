using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObjects/ItemData")]
public abstract class ItemScriptableObject : ScriptableObject
{
    public Sprite sprite;
    public int maxStack = 1;

    public abstract bool use();
}

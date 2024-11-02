using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

#region Interfaces
public interface IUsable
{
    bool Use(); 
}
#endregion


#region ScriptableObject Base Class
[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObjects/ItemData")]
public abstract class ItemScriptableObject : ScriptableObject, IUsable
{
    public Sprite sprite; // Icon of the item
    public int id = -1; // Unique identifier of the item.
    // ID MUST BE UNIQUE FOR EACH ITEM : please write its unique id
    // down into this shared document: https://docs.google.com/spreadsheets/d/1KaqWPhWwRt81qpsXpCp1f_tODKXNXuQClTnDTaj015U/edit?usp=sharing
    // [You may initially find it a bit troublesome, but when it comes to room generation and filling treasure chest contents, this method will greatly simplify the workload in the end.]
    public string itemName = ""; // Name of the item
    public string itemDescription = ""; // Description of the item
    public bool consumable = false; // Indicates whether the item is consumable

    public virtual bool Use() { return true; } // Default implementation of the Use method
    public int maxStack = 1; // Maximum stack limit
}
#endregion


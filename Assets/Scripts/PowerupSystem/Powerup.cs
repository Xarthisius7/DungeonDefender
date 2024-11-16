using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Powerup", menuName = "Powerups/Powerup", order = 1)]
public class Powerup : ScriptableObject
{
    public List<AttributeBoost> boosts; // List of attribute boosts
    public bool isMagic;                // True if it's a Magic type powerup
    public int id;                      // PowerUp ID
    public Sprite sprite;               // Powerup icon
    public string description;          // Powerup effect description
}
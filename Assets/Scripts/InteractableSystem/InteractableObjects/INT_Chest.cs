using UnityEngine;

public class INT_Chest : MonoBehaviour, IInteractable
{
    public Sprite openedSprite;
    public void OnInteraction()
    {
        Debug.Log("You opened the chest!");
        // TODO: Chest opening events

        gameObject.GetComponent<SpriteRenderer>().sprite = openedSprite; 
    }
}

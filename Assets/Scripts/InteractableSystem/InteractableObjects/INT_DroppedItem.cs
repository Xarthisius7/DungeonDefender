using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class INT_DroppedItem : MonoBehaviour, IInteractable
{

    public ItemScriptableObject item;
    public int amount;

    public SpriteRenderer sp;


    public void setItem(ItemScriptableObject i, int itemAmount)
    {
        item = i;
        this.amount = itemAmount;

        sp.sprite = item.sprite;

    }

    public void OnInteraction()
    {

        ItemManager.Instance.AddItemsById(item.id, amount);

        Debug.Log("You Picked up the item!");


        //// if successfully picked up the item:
        //InteractionTrigger interactionTrigger = GetComponent<InteractionTrigger>();
        //if (interactionTrigger != null)
        //{
        //    // Call the DisableButton() method
        //    interactionTrigger.DisableButton();
        //}
        Destroy(gameObject);
      

    }

    void Start()
    {
        if(item != null)
        {
            sp.sprite = item.sprite;
        }
    }

}

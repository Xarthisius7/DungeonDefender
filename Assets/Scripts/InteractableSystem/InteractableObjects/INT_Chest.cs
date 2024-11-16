using UnityEngine;

public class INT_Chest : MonoBehaviour, IInteractable
{
    public Sprite openedSprite;
    public int type;
    //TYPE:
    //0: small chest, unlocked
    //1: small chest, locked
    //2: large chest, unlocked
    //3: large chest, locked
    //4: spell chest(ability), locked.



    public void OnInteraction()
    {
        Debug.Log("You opened the chest!");
        // TODO: Chest opening events

        gameObject.GetComponent<SpriteRenderer>().sprite = openedSprite;


        InteractionTrigger interactionTrigger = GetComponent<InteractionTrigger>();
        if (interactionTrigger != null)
        {
            // Call the DisableButton() method
            interactionTrigger.DisableButton();
        }


        Transform shadowBorder = transform.Find("ShadowBorder");
        if (shadowBorder != null)
        {
            Destroy(shadowBorder.gameObject);
        }
        Transform shadowBorder2 = transform.Find("ShadowBorder2");
        if (shadowBorder != null)
        {
            shadowBorder2.gameObject.SetActive(true);
        }
    }
}

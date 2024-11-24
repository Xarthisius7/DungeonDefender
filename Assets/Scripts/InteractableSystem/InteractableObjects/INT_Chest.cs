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
    //5: Story chest: trigger story. Give player 2 keys.



    public void OnInteraction()
    {

        switch (type)
        {
            case 0:
                {
                    UIManager.Instance.ShowMessage("You opened a small chest...");

                    smallChestUnlocked();
                    EffectsManager.Instance.PlaySFX(7);
                    OpenChest();
                    break;
                }

            case 1:
                {

                    if (ItemManager.Instance.ConsumeItemById(16))
                    {
                        //if player have the key.

                        UIManager.Instance.ShowMessage("You opened a small Treasure chest...");
                        EffectsManager.Instance.PlaySFX(14);
                        InteractionTrigger interactionTrigger = GetComponent<InteractionTrigger>();
                        if (interactionTrigger != null)
                        {
                            // Call the DisableButton() method
                            interactionTrigger.DisableButton();
                        }

                        Invoke("smallLockedChest", 1f);
                    } else
                    {

                        UIManager.Instance.ShowMessage("You dont have the Key!");
                    }


                    break;
                }

            case 2:
                UIManager.Instance.ShowMessage("You opened a Large Chest!");

                smallChestUnlocked();
                smallChestUnlocked();
                smallChestUnlocked();
                EffectsManager.Instance.PlaySFX(7);
                OpenChest();
                break;

            case 3:

                if (ItemManager.Instance.ConsumeItemById(16))
                {
                    //if player have the key.
                    UIManager.Instance.ShowMessage("You opened a Large Treasure Chest!");
                    EffectsManager.Instance.PlaySFX(14);
                    InteractionTrigger interactionTrigger = GetComponent<InteractionTrigger>();
                    if (interactionTrigger != null)
                    {
                        // Call the DisableButton() method
                        interactionTrigger.DisableButton();
                    }

                    Invoke("LargeLockedChest", 1f);
                }
                else
                {

                    UIManager.Instance.ShowMessage("You dont have the Key!");
                }

                break;

            case 4:
                if (ItemManager.Instance.ConsumeItemById(16))
                {
                    //if player have the key.
                    UIManager.Instance.ShowMessage("You opened a Ability Treasure Chest!");
                    EffectsManager.Instance.PlaySFX(14);
                    InteractionTrigger interactionTrigger = GetComponent<InteractionTrigger>();
                    if (interactionTrigger != null)
                    {
                        // Call the DisableButton() method
                        interactionTrigger.DisableButton();
                    }

                    Invoke("AbilityChest", 1f);
                }
                else
                {

                    UIManager.Instance.ShowMessage("You dont have the Key!");
                }
                break;
            case 5:
                EffectsManager.Instance.PlaySFX(22);
                InteractionTrigger interactionTrigger2 = GetComponent<InteractionTrigger>();
                if (interactionTrigger2 != null)
                {
                    // Call the DisableButton() method
                    interactionTrigger2.DisableButton();
                }
                ItemManager.Instance.AddItemsById(
                    16, 2
                    );
                //TODO: Trigger the story Event


                break;
            default:
                Debug.LogWarning("Unknown chest type. Please check the 'type' variable.");
                break;
        }



    }



    private void AbilityChest()
    {

        GameController.Instance.PauseGame();

        UIManager.Instance.OpenItemMenuFunc(PowerupManager.instance.LoadRandomPowerups(), PowerupManager.instance.LoadRandomPowerups(), PowerupManager.instance.LoadRandomPowerups());


        EffectsManager.Instance.PlaySFX(7);
        EffectsManager.Instance.PlaySFX(15);
        OpenChestWithoutTrigger();
    }


    private void smallChestUnlocked()
    {

        float prob1 = 90f; // for common item
        float prob2 = 5f; // for equipment
        float prob3 = 5f;  // for key - just a notice. can be ignored.
        float randomValue = Random.Range(0f, 100f);

        if (randomValue < prob1)
        {
            //common item
            ItemManager.Instance.AddItemsById(
                ItemManager.Instance.GetRandomConsumeableID(), 1
                );
        }
        else if (randomValue < prob1 + prob2)
        {
            //equipment
            ItemManager.Instance.AddItemsById(
                ItemManager.Instance.GetRandomEquipmentID(), 1
                );
        }
        else
        {
            //key 
            ItemManager.Instance.AddItemsById(
                16, 1
                );
        }
    }

    private void smallLockedChest()
    {

        float prob1 = 90f; // for common item
        float prob2 = 10f; // for equipment


        float prob3 = 35f;  // for 3 choose 1 powerup scroll
        float randomValue = Random.Range(0f, 100f);

        if (randomValue < prob1)
        {
            //common item
            ItemManager.Instance.AddItemsById(
                ItemManager.Instance.GetRandomConsumeableID(), 1
                );
        }
        else
        {
            //equipment
            ItemManager.Instance.AddItemsById(
                ItemManager.Instance.GetRandomEquipmentID(), 1
                );
        }

        float randomValue2 = Random.Range(0f, 100f);
        if (randomValue2 < prob3)
        {
            //powerup scroll
            ItemManager.Instance.AddItemsById(
                19, 1
                );
        }
        else
        {
            ItemManager.Instance.AddItemsById(
                20, 1
                );
        }

        EffectsManager.Instance.PlaySFX(7);
        OpenChestWithoutTrigger();
    }
    
    private void LargeLockedChest()
    {

        float prob1 = 90f; // for common item
        float prob2 = 10f; // for equipment

        for (int i = 0; i < 2; i++) {

            float randomValue = Random.Range(0f, 100f);

            if (randomValue < prob1)
            {
                //common item
                ItemManager.Instance.AddItemsById(
                    ItemManager.Instance.GetRandomConsumeableID(), 2
                    );
            }
            else
            {
                //equipment
                ItemManager.Instance.AddItemsById(
                    ItemManager.Instance.GetRandomEquipmentID(), 2
                    );
            }
        }

        ItemManager.Instance.AddItemsById(
            19, 1
            );
        ItemManager.Instance.AddItemsById(
            20, 1
            );

        EffectsManager.Instance.PlaySFX(7);
        EffectsManager.Instance.PlaySFX(15);
        OpenChestWithoutTrigger();
    }

    private void OpenChestWithoutTrigger()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = openedSprite;



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

    private void OpenChest()
    {
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

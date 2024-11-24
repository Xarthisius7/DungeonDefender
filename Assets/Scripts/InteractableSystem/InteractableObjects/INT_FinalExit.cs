using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class INT_FinalExit : MonoBehaviour, IInteractable
{
    public GameObject[] gameObjects; 
    public GameObject DoorImage; 
    public Sprite changeImage;

    public void OnInteraction()
    {
        InteractionTrigger interactionTrigger = GetComponent<InteractionTrigger>();
        if (interactionTrigger != null)
        {
            // Call the DisableButton() method
            interactionTrigger.DisableButton();
        }

        StartCoroutine(LightSequence());
    }

    private System.Collections.IEnumerator LightSequence()
    {
        yield return new WaitForSeconds(0.5f); 

        int i = 0; int max = GameController.Instance.TowerDefensed;
        foreach (var obj in gameObjects)
        {
            i++; if (i > max)
            {
                break;
            }

            if (obj.TryGetComponent(out Light2D light))
            {
                light.enabled = true;
            }
            EffectsManager.Instance.PlaySFX(16, 1f);

            if (obj.TryGetComponent(out SpriteRenderer spriteRenderer))
            {
                spriteRenderer.sprite = changeImage;
            }

            yield return new WaitForSeconds(1f); 
        }

        if (GameController.Instance.TowerDefensed < 3)
        {
            foreach (var obj in gameObjects)
            {
                if (obj.TryGetComponent(out Light2D light))
                {
                    light.enabled = false;
                }
            }
            UIManager.Instance.ShowMessage("You havn't complete defend all 3 Crystal...");
            EffectsManager.Instance.PlaySFX(18,0.9f);
            EffectsManager.Instance.PlaySFX(17,1f);


            InteractionTrigger interactionTrigger = GetComponent<InteractionTrigger>();
            if (interactionTrigger != null)
            {
                // Call the DisableButton() method
                interactionTrigger.ReDispalyButton();
            }
        }
        else
        {
            DoorImage.SetActive(true);

            foreach (var obj in gameObjects)
            {
                if (obj.TryGetComponent(out Light2D light))
                {
                    light.intensity = 1.5f;
                }
            }

            if (DoorImage.TryGetComponent(out Animator animator))
            {
                animator.enabled = true;
            }
            EffectsManager.Instance.PlaySFX(20);

            UIManager.Instance.ShowMessage("A mysterious portal emerged in the center of the room... ");
            yield return new WaitForSeconds(3f);
            UIManager.Instance.ShowMessage("Where could it lead? ");



        }
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class INT_CrystalBase : MonoBehaviour, IInteractable
{
    public int type;
    public WavesController wc;


    public void OnInteraction()
    {

        EffectsManager.Instance.PlaySFX(17);
        EffectsManager.Instance.PlaySFX(18);


        switch (GameController.Instance.CurrentArea)
        {
            case 1:
                EffectsManager.Instance.PlayBackgroundMusicSmooth(4);
                break;

            case 2:
                EffectsManager.Instance.PlayBackgroundMusicSmooth(5);
                break;

            case 3:
                EffectsManager.Instance.PlayBackgroundMusicSmooth(4);
                break;

            default:
                break;
        }


        InteractionTrigger interactionTrigger = GetComponent<InteractionTrigger>();
        interactionTrigger.DisableButton();

        Invoke("delayTriggerDefense", 5f);

    }


    private void delayTriggerDefense()
    {
        wc.StartDefense();

    }
}

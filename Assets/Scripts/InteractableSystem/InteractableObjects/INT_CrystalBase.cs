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
        GameController.Instance.StartDefenceAWave();

        InteractionTrigger interactionTrigger = GetComponent<InteractionTrigger>();
        interactionTrigger.DisableButton();

        Invoke("delayTriggerDefense", 5f);

    }


    private void delayTriggerDefense()
    {
        wc.StartDefense();

    }
}

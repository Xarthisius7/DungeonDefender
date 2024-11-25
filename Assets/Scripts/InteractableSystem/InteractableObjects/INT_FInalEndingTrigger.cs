using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class INT_FInalEndingTrigger : MonoBehaviour, IInteractable
{
    int nbLores;
    public void OnInteraction()
    {
        //TODO: TOM : ADD your code that link to the game ending here
        nbLores = VNSceneManager.Instance.loreLearned;

        if (nbLores < 8)
        {
            //EndingA
            return;
        }
            
        else if (nbLores >= 8 && nbLores < 19)
        {
            //EndingB
            return;
        }

        else
        {
            //EndingC
            return;
        }

    }

}

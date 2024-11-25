using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class INT_FInalEndingTrigger : MonoBehaviour, IInteractable
{
    int nbLores;
    public void OnInteraction()
    {
        //TODO: TOM : ADD your code that link to the game ending here
        nbLores = VNSceneManager.Instance.loreLearned;

        if (nbLores < 8)
        {
            SceneManager.LoadScene(6);
        }

        else if (nbLores >= 8 && nbLores < 19)
        {
            SceneManager.LoadScene(7);
        }

        else
        {
            SceneManager.LoadScene(8);
        }

    }

}

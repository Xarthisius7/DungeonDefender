using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class INT_TutorialTeleporter : MonoBehaviour, IInteractable
{
    public AudioClip teleportSound;

    public GameObject PlayerObject;
    public void OnInteraction()
    {
        VNSceneManager.Instance.NVSound.clip = teleportSound;
        VNSceneManager.Instance.NVSound.Play();

        PlayerObject.transform.position = new Vector3(0.0f, 2.0f, 0.0f);
        VNSceneManager.Instance.isTutorial = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoreTrigger : MonoBehaviour, IInteractable
{
    public AudioClip PaperSound;

    
    public void OnInteraction()
    {
        VNSceneManager.Instance.NVSound.clip = PaperSound;
        VNSceneManager.Instance.NVSound.Play();
        VNSceneManager.Instance.StartLoreScene();
        Debug.Log("Lore Scene Triggered!!");
        Destroy(gameObject);
    }
}

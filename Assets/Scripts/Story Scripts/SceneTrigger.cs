using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Scene Trigger Trigged!");
        if (other.gameObject.tag == "Player")
        {
            VNSceneManager.Instance.VNSceneInProgress = true;
            Debug.Log("Trigger for Scene #" + VNSceneManager.Instance.SceneNb + " as been activated");
            Destroy(gameObject);

        }
    }
}

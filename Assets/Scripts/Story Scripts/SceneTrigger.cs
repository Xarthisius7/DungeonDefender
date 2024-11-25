using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Scene Trigger Trigged!");
        if (other.gameObject.tag == "Player" && VNSceneManager.Instance.SceneNb == 4)
        {
            VNSceneManager.Instance.StartStoryScene();
            Destroy(gameObject);
        }

        //To uncomment when testing other scenes
        //VNSceneManager.Instance.StartStoryScene();
        //Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Tutorial Trigger Trigged!");
        if (other.gameObject.tag == "Player")
        {
            VNSceneManager.Instance.StartTutorialScene();
            Destroy(gameObject);

        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScript : MonoBehaviour
{
    [SerializeField] public Text losingText;

    // Setup/enable the game over screen
    public void Setup(string losingCondition){
        gameObject.SetActive(true);
        losingText.text = losingCondition;
    }
    // set the game over screen to inactive
    public void remove(string losingCondition){
        gameObject.SetActive(true);
        losingText.text = losingCondition;
    }
}

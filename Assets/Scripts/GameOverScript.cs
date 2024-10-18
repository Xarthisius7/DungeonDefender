using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScript : MonoBehaviour
{
    public Text losingText;
    

    public void Setup(string losingCondition){
        gameObject.SetActive(true);
        losingText.text = losingCondition;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScript : MonoBehaviour
{
    // public static GameOverScript Instance { get; private set; }
    public Text losingText;
    
    // void Start()
    // {
    //     if (Instance == null)
    //     {
    //         Instance = this;
    //         DontDestroyOnLoad(gameObject);
    //     }
    //     else
    //     {
    //         Destroy(gameObject);
    //     }
    // }



    public void Setup(string losingCondition){
        gameObject.SetActive(true);
        losingText.text = losingCondition;
    }
}

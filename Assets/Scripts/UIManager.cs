using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameController;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }


    void Start()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }


    }

    void Update()
    {
        
    }

    public void BroadCast(string message)
    {
        // broadCast a message on screen
        // each message will going up 1 tile after a new one appears, and fades out aftertime. 
        // can be called from any system.
    }

    public void ShowPauseScreen()
    {

    }
    public void ClosePauseScreen()
    {

    }


    public void ShowMainMenu()
    {

    }

    public void CloseMainMenu()
    {

    }

    public void UpdateHealth(float health)
    {
        
    }

    public void UpdateStamina(float Stamina)
    {

    }


}

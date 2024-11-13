using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameController;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }


    [SerializeField] Image HealthBar;
    [SerializeField] Image StaminaBar;

    [SerializeField] float fillSpeed;


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
        PauseMenu pauseMenu = FindAnyObjectByType<PauseMenu>();

        if(pauseMenu != null)
        {
            pauseMenu.ShowPauseScreen();
        }
        else
        {
            Debug.LogWarning("No pause screen Found");
        }
    }
    public void ClosePauseScreen()
    {
        PauseMenu pauseMenu = FindAnyObjectByType<PauseMenu>();

        if (pauseMenu != null)
        {
            pauseMenu.ClosePauseScreen();
        }
        else
        {
            Debug.LogWarning("No pause screen Found");
        }
    }


    public void ShowMainMenu()
    {

    }

    public void CloseMainMenu()
    {

    }

    public void UpdateHealth(float targetPercentage)
    {
        StopCoroutine("SmoothHealthChange");
        StartCoroutine(SmoothHealthChange(targetPercentage));
    }

    public void UpdateStamina(float percantange)
    {
        StaminaBar.fillAmount = percantange;
    }

    private IEnumerator SmoothHealthChange(float targetPercentage)
    {
        float initialPercentage = HealthBar.fillAmount;
        float elapsedTime = 0f;
        float duration = Mathf.Abs(targetPercentage - initialPercentage) / fillSpeed;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            // Apply cubic easing for ease-in and ease-out
            t = t * t * (3f - 2f * t);
            HealthBar.fillAmount = Mathf.Lerp(initialPercentage, targetPercentage, t);
            yield return null;
        }

        HealthBar.fillAmount = targetPercentage;
    }

}

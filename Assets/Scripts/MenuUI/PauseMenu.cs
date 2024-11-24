using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Required for UI components

public class PauseMenu : MonoBehaviour
{
    private CanvasGroup pauseMenuCanvasGroup;
    private bool isPaused = false;
    public bool isHidden { get; private set; }

    [SerializeField] private Button resumeButton;
    [SerializeField] private Button returnToMenuButton;
    [SerializeField] private Button statsButton;

    void Awake()
    {
        // Get the CanvasGroup component attached to this GameObject
        pauseMenuCanvasGroup = GetComponent<CanvasGroup>();

        // Ensure the pause menu is hidden at the start
        pauseMenuCanvasGroup.alpha = 0;
        pauseMenuCanvasGroup.interactable = false;
        pauseMenuCanvasGroup.blocksRaycasts = false;

        // Set up the click listeners for the buttons
        resumeButton.onClick.AddListener(ClosePauseScreen);
        //returnToMenuButton.onClick.AddListener(ReturnToMenu);
        statsButton.onClick.AddListener(DisplayStats);

        isHidden = false;
    }

    private void DisplayStats()
    {
        HidePauseScreen(true);
        UIManager.Instance?.OpenAttrubuteMenu();
    }

    void Update()
    {
    }

    public void ShowPauseScreen()
    {
        HidePauseScreen(false);

        isPaused = true;
        pauseMenuCanvasGroup.alpha = 1;
        pauseMenuCanvasGroup.interactable = true;
        pauseMenuCanvasGroup.blocksRaycasts = true;
    }

    public void ClosePauseScreen()
    {
        HidePauseScreen(false);

        Time.timeScale = 1f; // Ensure the game is not paused
        isPaused = false;
        pauseMenuCanvasGroup.alpha = 0;
        pauseMenuCanvasGroup.interactable = false;
        pauseMenuCanvasGroup.blocksRaycasts = false;

        if (GameController.Instance != null)
        {
            GameController.Instance.IsPaused = false;
        }
        else
        {
            Debug.Log("No game controller found");
        }
    }

    public void HidePauseScreen(bool hide)
    {
        isHidden = hide;
        if(isHidden)
        {
            pauseMenuCanvasGroup.alpha = 0;
            pauseMenuCanvasGroup.interactable = false;
            pauseMenuCanvasGroup.blocksRaycasts = false;
        }

        else
        {
            pauseMenuCanvasGroup.alpha = 1;
            pauseMenuCanvasGroup.interactable = true;
            pauseMenuCanvasGroup.blocksRaycasts = true;

            UIManager.Instance?.CloseAttrubuteMenu();

            SettingsPanel settings = FindAnyObjectByType<SettingsPanel>();

            settings?.CloseSettingsPanel();
        }
    }

    private void ReturnToMenu()
    {
        Time.timeScale = 1f; // Ensure the game is not paused
        SceneGameManager.Instance.ReturnToMenu();
    }
}

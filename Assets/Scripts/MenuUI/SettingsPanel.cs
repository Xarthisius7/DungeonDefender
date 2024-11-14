using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Required for UI components

public class SettingsPanel : MonoBehaviour
{
    private CanvasGroup settingsPanelCanvasGroup;

    [SerializeField] private Button settingsButton; // The button that opens the settings panel
    [SerializeField] private Button closeButton; // The button that closes the settings panel
    [SerializeField] private Slider soundfxSlider; // The slider that controls the volume
    [SerializeField] private Slider musicSlider; // The slider that controls the volume

    private SceneGameManager sceneGameManager; // Reference to the SceneGameManager

    void Awake()
    {
        // Get the CanvasGroup component attached to the settings panel
        settingsPanelCanvasGroup = GetComponent<CanvasGroup>();

        // Ensure the settings panel is hidden at the start
        settingsPanelCanvasGroup.alpha = 0;
        settingsPanelCanvasGroup.interactable = false;
        settingsPanelCanvasGroup.blocksRaycasts = false;

        // Set up the click listeners
        settingsButton.onClick.AddListener(OpenSettingsPanel);
        closeButton.onClick.AddListener(CloseSettingsPanel);

        // Find the SceneGameManager in the scene
        sceneGameManager = FindObjectOfType<SceneGameManager>();

        // Set up the slider's initial value and listener
        if (sceneGameManager != null)
        {
            soundfxSlider.value = sceneGameManager.GameFXVolume; // Set the slider to the current volume
            soundfxSlider.onValueChanged.AddListener(UpdateFXVolume); // Listen for value changes

            musicSlider.value = sceneGameManager.GameMusicVolume; // Set the slider to the current volume
            musicSlider.onValueChanged.AddListener(UpdateMusicVolume); // Listen for value changes
        }
    }

    private void OpenSettingsPanel()
    {
        settingsPanelCanvasGroup.alpha = 1;
        settingsPanelCanvasGroup.interactable = true;
        settingsPanelCanvasGroup.blocksRaycasts = true;
    }

    private void CloseSettingsPanel()
    {
        settingsPanelCanvasGroup.alpha = 0;
        settingsPanelCanvasGroup.interactable = false;
        settingsPanelCanvasGroup.blocksRaycasts = false;
    }

    private void UpdateFXVolume(float value)
    {
        if (sceneGameManager != null)
        {
            sceneGameManager.GameFXVolume = value; // Update the volume variable in the SceneGameManager
        }
    }

    private void UpdateMusicVolume(float value)
    {
        if (sceneGameManager != null)
        {
            sceneGameManager.GameMusicVolume = value; // Update the volume variable in the SceneGameManager
        }
    }
}
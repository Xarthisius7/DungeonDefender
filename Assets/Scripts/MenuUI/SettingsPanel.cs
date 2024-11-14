using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Required for UI components

public class SettingsPanel : MonoBehaviour
{
    private CanvasGroup settingsPanelCanvasGroup;

    [SerializeField] private Button settingsButton; // The button that opens the settings panel
    [SerializeField] private Button closeButton; // The button that closes the settings panel
    [SerializeField] private Slider volumeSlider; // The slider that controls the volume

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
            volumeSlider.value = sceneGameManager.GameVolume; // Set the slider to the current volume
            volumeSlider.onValueChanged.AddListener(UpdateVolume); // Listen for value changes
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

    private void UpdateVolume(float value)
    {
        if (sceneGameManager != null)
        {
            sceneGameManager.GameVolume = value; // Update the volume variable in the SceneGameManager
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class SceneGameManager : MonoBehaviour
{
    public static SceneGameManager Instance;
    CanvasGroup blackCanvas;
    public event EventHandler LevelStarted;
    public event EventHandler MenuEnter;

    public float GameFXVolume = 1.0f;
    public float GameMusicVolume = 1.0f;

    [SerializeField] string menuScene;
    [SerializeField] string introScene;
    [SerializeField] string gameScene;
    [SerializeField] string endingScene;
    [SerializeField] string creditsScene;

    public enum SceneType
    {
        Menu,
        Intro,
        Game,
        Ending,
        Credits
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist between scenes
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
        }
    }

    public void LoadScene(SceneType sceneType)
    {
        StartCoroutine(LoadSceneCoroutine(sceneType));
    }

    private IEnumerator LoadSceneCoroutine(SceneType sceneType)
    {
        string sceneName = GetSceneName(sceneType);

        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Invalid scene type or scene name not set!");
            yield break;
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Trigger specific events for certain scenes if needed
        if (sceneType == SceneType.Menu)
        {
            MenuEnter?.Invoke(this, EventArgs.Empty);
        }
        else if (sceneType == SceneType.Game)
        {
            LevelStarted?.Invoke(this, EventArgs.Empty);
        }
    }

    private string GetSceneName(SceneType sceneType)
    {
        return sceneType switch
        {
            SceneType.Menu => menuScene,
            SceneType.Intro => introScene,
            SceneType.Game => gameScene,
            SceneType.Ending => endingScene,
            SceneType.Credits => creditsScene,
            _ => null,
        };
    }

    private IEnumerator FadeToBlack()
    {
        yield return new WaitForSeconds(0.2f);

        float elapsedTime = 0f;
        float fadeDuration = 0.5f;

        while (elapsedTime < fadeDuration)
        {
            blackCanvas.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        blackCanvas.alpha = 1f;

        yield return null;
    }
}
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

    [SerializeField] string gameScene;

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

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReturnToMenu()
    {
        StartCoroutine(LoadMenuScene());
    }

    private IEnumerator LoadMenuScene()
    {
        blackCanvas = FindFirstObjectByType<CanvasGroup>();

        if(blackCanvas != null)
        {
            yield return FadeToBlack();
        }

        // Assuming "BattleScene" is the name of the battle scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("GameMenu");

        // Wait until the scene is fully loaded
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        MenuEnter?.Invoke(this, EventArgs.Empty);
    }

    public void TransitionToGame()
    {
        StartCoroutine(LoadGameScene());
    }

    private IEnumerator LoadGameScene()
    {
        blackCanvas = FindFirstObjectByType<CanvasGroup>();

        if (blackCanvas != null)
        {
            yield return FadeToBlack();
        }

        // Assuming "BattleScene" is the name of the battle scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(gameScene);

        // Wait until the scene is fully loaded
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        LevelStarted?.Invoke(this, EventArgs.Empty);
    }

    private IEnumerator FadeToBlack()
    {
        yield return new WaitForSeconds(0.2f);

        float elapsedTime = 0f;
        float fadeDuration = 0.5f;

        while (elapsedTime < fadeDuration)
        {
            // Incrementally increase the alpha of the canvas group to create a fading effect
            blackCanvas.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the screen is fully black at the end
        blackCanvas.alpha = 1f;

        yield return null;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The GameController class manages the overall game flow, including
/// transitions between game states like MainMenu, Exploration, Defense, and GameOver.
/// </summary>
public class GameController : MonoBehaviour
{
    // Reference to other managers
    [SerializeField] public MapManager mapManager;
    [SerializeField] public PlayerController playerController;
    [SerializeField] public EnemyManager enemyManager;
    [SerializeField] public BaseDefenseManager baseDefenseManager;
    [SerializeField] public PowerupManager powerUpManager;
    [SerializeField] public ItemManager itemManager;
    [SerializeField] public UIManager uiManager;
    [SerializeField] public EffectsManager effectsManager;

    public static GameController Instance { get; private set; }

    // Game states
    public enum GameState { MainMenu, Exploration, Defense, GameOver }
    private GameState currentState;

    void Start()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Keep the game controller exict when loading the scence
        }
        else
        {
            Destroy(gameObject); 
        }


        // Initialize the game state
        currentState = GameState.MainMenu;
        InitGame();


    }

    void Update()
    {
        // Pause & Resume Game Using Escape Key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 1)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }


        // Handle state transitions and updates
        switch (currentState)
        {
            case GameState.Exploration:
                // Update exploration logic
                break;
            case GameState.Defense:
                // Update defense logic
                break;
            case GameState.GameOver:
                // Game over logic
                break;
        }
    }

    /// <summary>
    /// Pause the Game, and show the Pause Menu.
    /// </summary>
    public void PauseGame()
    {
        Time.timeScale = 0;
        UIManager.Instance.ShowPauseScreen();
        Debug.Log("Game Paused.");
    }

    /// <summary>
    /// Resume the Game, and Hide the Pause Menu.
    /// </summary>
    public void ResumeGame()
    {
        Time.timeScale = 1;
        Debug.Log("Game Resumed.");
        UIManager.Instance.ClosePauseScreen();
    }



    /// <summary>
    /// Changes the current game state to the specified new state.
    /// </summary>
    /// <param name="newState">The new game state to transition to.</param>
    public void ChangeGameState(GameController.GameState newState)
    {
        currentState = newState;
        Debug.Log("Game state has changed to: " + currentState);
    }


    // Initialize game logic, link managers, set up the map
    private void InitGame()
    {
        // Set up the map and player starting positions - TODO After finishing map generation
        //mapManager.GenerateMap();
        //playerController.SpawnPlayerAt(mapManager.GetStartPosition());

        // Initialize UI and other systems
        //uiManager.ShowMainMenu();
        //effectsManager.PlayBackgroundMusic();
    }

    // Transition to exploration phase
    public void StartExploration()
    {
        currentState = GameState.Exploration;
        //uiManager.HideMainMenu();
        //playerController.EnablePlayerControls();
    }


    // End game when conditions are met
    public void GameOver()
    {
        currentState = GameState.GameOver;
        //uiManager.ShowGameOverScreen();
        //effectsManager.PlayGameOverSound();
    }
}

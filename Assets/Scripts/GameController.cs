using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using static UnityEngine.EventSystems.EventTrigger;

/// <summary>
/// The GameController class manages the overall game flow, including
/// transitions between game states like MainMenu, Exploration, Defense, and GameOver.
/// </summary>
/// 



public class RoomStatus
{
    public int hasVisited = 0;

    public bool hasLightUped = false;

    public RoomStatus()
    {
        this.hasVisited = 0;
        this.hasLightUped = false;
    }
}
public class GameController : MonoBehaviour
{

    [SerializeField] public Transform playerTransform;

    [SerializeField] public int OverallDifficulty = 1;
    //Overall difficulty.  0: easy. 1: normal. 2: hard.
    //default set to 1.  it can be changed from game Setting.

    public int CurrentDifficulty = 1;
    public int CurrentArea = 1;
    public int PreviousArea = 1;
    private float innerDifficuities = 1;
    private float roomDifficuitiesIncrease = 0.02f;
    private float areaDifficuitiesIncrase = 2;

    public bool isDefencing = false;

    public bool IsGameRunning = true;
    //Is game still running - game over will effect this.
    public bool IsPaused = true;

    private GridCell[,] grid;
    private RoomStatus[,] hasVisited;
    private GameObject[,] mapPiecesToToggle;
    private Transform MapPiecesTransform;

    public int TowerDefensed = 0;
    //the count 3 crystal player needs to defend.

    public float Area1BaseDiff = 1;
    public float Area2BaseDiff = 4;
    public float Area3BaseDiff = 9;
    public float Area4BaseDiff = 15;
    public int roomsEplored= 0;

    private int centerX = 0;
    private int centerY = 0;
    private Transform mapCenter;

    bool hasPaused = false;


    private float elapsedTime = 0f; 

    public WavesController currentTower;

    public static GameController Instance { get; private set; }

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


        Invoke("InitGame", 0.1f);


    }


    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }



    public void CloseAllMenu()
    {
        // Close all menu - stats menu, pause menu, powerup choosing menu, etc. 
        // Call this before opening any menu.
        //TODO: Add Pause Screen menu, etc

        UIManager.Instance.ChooseOption(1);//powerup choosing menu
        UIManager.Instance.CloseAttrubuteMenu();//Stats menu
    }


    // Initialize game logic, link managers, set up the map
    private void InitGame()
    {
        //INIT GAME PLAN:
        // 1. play a short CG / background story nerrator
        // 2. init the game and puase it
        // 3. unpause it after the CG is over.

        //TODO:  1. play a short CG / background story nerrator


        EffectsManager.Instance.PlayBackgroundMusic(1);

        // 2. init the game and puase it
        grid = MapManager.Instance.CreateMap();
        mapPiecesToToggle = MapManager.Instance.MakeMiniMap();
        MapPiecesTransform = GameObject.FindWithTag("MapPiecesList").transform;

        centerX = MapManager.Instance.StartRoomX;
        centerY = MapManager.Instance.StartRoomY;
        mapCenter = grid[centerX, centerY].roomObject.transform;

        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);
        hasVisited = new RoomStatus[rows, cols];



        //init the hasVisited, which records if player has visited that room.
        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < cols; y++)
            {
                hasVisited[x, y] = new RoomStatus();
                if (grid[x, y].hasRoom)
                {
                    hasVisited[x, y].hasVisited = 1; // Set the location that has room to 1 (to be visit) ; 2 => visited
                    mapPiecesToToggle[x, y].SetActive(false);
                }
            }
        }


        IsGameRunning = true;
        IsPaused = false;

        // Initialize UI and other systems
        //uiManager.ShowMainMenu();
        //effectsManager.PlayBackgroundMusic();


        Invoke("GameInitDelayTerms", 0.1f);


    }


    private void GameInitDelayTerms()
    {
        //things that happens after game start. 
    }


    void Update()
    {

        float pauseKeyDown = InputManager.IsPausingGame;

        

        // Pause & Resume Game Using Escape Key
        if (pauseKeyDown > 0 && IsGameRunning && !hasPaused)
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

        if (pauseKeyDown > 0)
            hasPaused = true;
        else
            hasPaused = false;

        if (!IsPaused && !VNSceneManager.Instance.isTutorial)
        {
            //if the game is still running, constanting check the player's position.
            //and trigger the needed events.(e.g. spawning enemies)
            UpdatePlayerRoom();

            elapsedTime += Time.deltaTime; 
        }

    }

    public void StartDefenceAWave(WavesController wc)
    {
        //MUST BE CALLED when player start defencing. 


        isDefencing = true;
        currentTower = wc;

    }

    public int PlayerFinishedDefense()
    {
        TowerDefensed++;
        isDefencing = false;
        UIManager.Instance.UpdateCrystalsDisplay(TowerDefensed);
        ChangeBGMBaseOnArea();

        roomsEplored = 0;
        return TowerDefensed;
    }

    public bool TryToActivatedExitRoom()
    {
        // Final room's activiation - check if the game goal is done.
        if (TowerDefensed >= 3)
        {
            //TODO : trigger Ending
            Debug.Log("Player try to activate the final crystal's exit - to escape the room. result is: true");
            return true;
        }
        else
        {
            Debug.Log("Player try to activate the final crystal's exit - to escape the room. result is: false");
            return false;
        }
    }

    public void DefenseFailed()
    {
        // player failed to defense. trigger GameOver.

        //TODO : change bgm, etc..
        isDefencing = false;
        GameOver();
    }
    public void PlayerDeath()
    {
        // player Dead in combat. trigger GameOver.

        //TODO : change bgm, etc..

        GameOver();
    }




    private void UpdatePlayerRoom()
    {
        // Calculate the distance between player and the map center
        Vector3 distance = playerTransform.position - mapCenter.position;

        // Calculate the room difference in x and y
        int diffX = Mathf.RoundToInt(distance.x / 8.0f);
        int diffY = Mathf.RoundToInt(distance.y / 9.0f);

        // Calculate the player's current room indices based on the center room indices
        int playerRoomX = centerX + diffX;
        int playerRoomY = centerY + diffY;

        if(grid[playerRoomX, playerRoomY].hasRoom)
        {
            updateDifficulty(4-(grid[playerRoomX, playerRoomY].Level));
        }

        if (hasVisited[playerRoomX, playerRoomY].hasVisited == 1)
        {
            hasVisited[playerRoomX, playerRoomY].hasVisited = 2;
            //If that room havn't been visited: trigger roon enter event. 
            //1. Spawn enemies. TODO

            List<Transform> spawnPoints = new List<Transform>(); // stores all the spawns points
            Transform[] allChildren = grid[playerRoomX, playerRoomY].roomObject.GetComponentsInChildren<Transform>();
            foreach (Transform child in allChildren)
            {
                string childName = child.gameObject.name;

                if (childName.Contains("EnemySpawnPoint") ||
                    childName.Contains("ShowEnemySpawnPoint") ||
                    childName.Contains("ShowEnemySpawnPointLlight"))
                {
                    spawnPoints.Add(child); // find by name.
                }
            }

            GameObject enemy1 = EnemyManager.Instance.GetRandomEnemy();
            GameObject enemy2 = null;

            // 50% chance spawning 2 types of enemy in a room
            if (Random.value > 0.5f)
            {
                do
                {
                    enemy2 = EnemyManager.Instance.GetRandomEnemy();
                } while (enemy2 == enemy1); 
            }

            bool useFirstEnemy = true; 

            foreach (Transform spawnPoint in spawnPoints)
            {
                spawnPoint.position = new Vector3(spawnPoint.position.x, spawnPoint.position.y, 0);

                // switching between type1 and type2 if there's 2 type
                GameObject enemyToSummon = (enemy2 != null && !useFirstEnemy) ? enemy2 : enemy1;
                EnemyManager.Instance.SummonEenemy(enemyToSummon, spawnPoint, CurrentDifficulty);

                useFirstEnemy = !useFirstEnemy;
            }

            // 2. increate the current difficulty
            roomsEplored += 1;

        }
        else if ((hasVisited[playerRoomX, playerRoomY].hasVisited == 2) && !hasVisited[playerRoomX, playerRoomY].hasLightUped)
        {
            //if the room has been visited by havn't been light up, test the distance,
            //if is within the range of 3, light up the room and mini map.
            Transform currentRoomTransform = grid[playerRoomX, playerRoomY].roomObject.transform;
            float distanceToRoomCenter = Vector3.Distance(playerTransform.position, currentRoomTransform.position);
            if (distanceToRoomCenter < 3f)
            {
                hasVisited[playerRoomX, playerRoomY].hasLightUped = true;
                mapPiecesToToggle[playerRoomX, playerRoomY].SetActive(true);


                Transform roomTransform = grid[playerRoomX, playerRoomY].roomObject.transform;
                Transform exploredLightTransform = roomTransform.Find("ExploredLight");

                if (exploredLightTransform != null)
                {
                    // find the  Light2D component
                    Light2D light2D = exploredLightTransform.GetComponent<Light2D>();
                    if (light2D != null)
                    {
                        light2D.enabled = true;
                        exploredLightTransform.gameObject.SetActive(true);

                        float targetIntensity = light2D.intensity;

                        // set the intensity to 0��and slowly towards the target value.
                        light2D.intensity = 0;
                        StartCoroutine(GraduallyIncreaseIntensity(light2D, targetIntensity, 10f));
                    }
                }


            }

        }


    }

    private void updateDifficulty(int Area)
    {
        int[] maxRoomsPerArea = { 9, 18, 27 };

        if (Area < 1 || Area > 3)
        {
            Debug.LogError("Invalid area value. Area must be 1, 2, or 3.");
            return;
        }

        float explorationFactor = (float)roomsEplored / maxRoomsPerArea[Area - 1];

        float currentBaseDiff = Area == 1 ? Area1BaseDiff : (Area == 2 ? Area2BaseDiff : Area3BaseDiff);
        float nextBaseDiff = Area == 1 ? Area2BaseDiff : (Area == 2 ? Area3BaseDiff : Area4BaseDiff);
        CurrentArea = Area;
        CurrentDifficulty = Mathf.RoundToInt(currentBaseDiff + explorationFactor * (nextBaseDiff - currentBaseDiff));

        if(CurrentArea != PreviousArea && !isDefencing)
        {
            PreviousArea = CurrentArea;
            ChangeBGMBaseOnArea();
        }
    }

    public void ChangeBGMBaseOnArea()
    {
        switch (CurrentArea)
        {
            case 1:
                EffectsManager.Instance.PlayBackgroundMusicSmooth(1);
                break;

            case 2:
                EffectsManager.Instance.PlayBackgroundMusicSmooth(2);
                break;

            case 3:
                EffectsManager.Instance.PlayBackgroundMusicSmooth(3);
                break;

            default:
                break;
        }
    }

    private IEnumerator GraduallyIncreaseIntensity(Light2D light, float targetIntensity, float duration)
    {
        float elapsed = 0;
        float initialIntensity = light.intensity;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            light.intensity = Mathf.Lerp(initialIntensity, targetIntensity, elapsed / duration);
            yield return null;
        }

        light.intensity = targetIntensity;
    }





    public void PauseGame()
    {
        //Pause the game.
        Time.timeScale = 0;
        IsPaused = true;
        UIManager.Instance.ShowPauseScreen();
        Debug.Log("Game Paused.");
    }


    public void ResumeGame()
    {
        PauseMenu pause = FindAnyObjectByType<PauseMenu>();

        if (pause != null && pause.isHidden)
        {
            pause.HidePauseScreen(false);
        }

        else
        {
            //Resume the game.
            Time.timeScale = 1;
            IsPaused = false;
            Debug.Log("Game Resumed.");
            UIManager.Instance.ClosePauseScreen();
        }
    }



    public void ShowVictory()
    {
        UIManager.Instance.ShowVictoryUI(FormatTime(elapsedTime));
    }

    // End game when conditions are met
    public void GameOver()
    {
        IsGameRunning = false;
        IsPaused = true;
        Debug.Log("Game Over! ");
        UIManager.Instance.ShowLosingUI();


    }

    public void RestartGame()
    {

        //TODO: restart game function
    }
}

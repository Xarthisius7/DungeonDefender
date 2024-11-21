using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;

public class VNSceneManager : MonoBehaviour
{
    

    #region Scenes&Dialogues
    private int nbScenes = 6;

    string[,] Scene1 = new string[5,3]
    {
        { "ChloeContent", "Chloe", "Dialogue 1 of Scene 1" } ,
        { "ChloeContent", "Chloe", "Dialogue 2 of Scene 1" },
        { "ChloeCurious", "Chloe", "Dialogue 3 of Scene 1" },
        { "ChloeContent", "Chloe", "Dialogue 4 of Scene 1" },
        { "ChloeCurious", "Chloe", "Dialogue 5 of Scene 1" }
    };

    string[,] Scene2 = new string[5, 3]
    {
        { "ChloeContent", "Chloe", "Dialogue 1 of Scene 2" } ,
        { "ChloeContent", "Chloe", "Dialogue 2 of Scene 2" },
        { "ChloeCurious", "Chloe", "Dialogue 3 of Scene 2" },
        { "ChloeContent", "Chloe", "Dialogue 4 of Scene 2" },
        { "ChloeCurious", "Chloe", "Dialogue 5 of Scene 2" }
    };

    string[,] Scene3 = new string[5, 3]
    {
        { "ChloeContent", "Chloe", "Dialogue 1 of Scene 3" } ,
        { "ChloeContent", "Chloe", "Dialogue 2 of Scene 3" },
        { "ChloeCurious", "Chloe", "Dialogue 3 of Scene 3" },
        { "ChloeContent", "Chloe", "Dialogue 4 of Scene 3" },
        { "ChloeCurious", "Chloe", "Dialogue 5 of Scene 3" }
    };

    string[,] Scene4 = new string[5, 3]
    {
        { "ChloeContent", "Chloe", "Dialogue 1 of Scene 4" } ,
        { "ChloeContent", "Chloe", "Dialogue 2 of Scene 4" },
        { "ChloeCurious", "Chloe", "Dialogue 3 of Scene 4" },
        { "ChloeContent", "Chloe", "Dialogue 4 of Scene 4" },
        { "ChloeCurious", "Chloe", "Dialogue 5 of Scene 4" }
    };

    string[,] Scene5 = new string[5, 3]
    {
        { "ChloeContent", "Chloe", "Dialogue 1 of Scene 5" } ,
        { "ChloeContent", "Chloe", "Dialogue 2 of Scene 5" },
        { "ChloeCurious", "Chloe", "Dialogue 3 of Scene 5" },
        { "ChloeContent", "Chloe", "Dialogue 4 of Scene 5" },
        { "ChloeCurious", "Chloe", "Dialogue 5 of Scene 5" }
    };

    string[,] Scene6 = new string[5, 3]
    {
        { "ChloeContent", "Chloe", "Dialogue 1 of Scene 6" } ,
        { "ChloeContent", "Chloe", "Dialogue 2 of Scene 6" },
        { "ChloeCurious", "Chloe", "Dialogue 3 of Scene 6" },
        { "ChloeContent", "Chloe", "Dialogue 4 of Scene 6" },
        { "ChloeCurious", "Chloe", "Dialogue 5 of Scene 6" }
    };
    #endregion

    DialogueSystem ds;
    TextArchitect architect;
    public GameObject CharacterOne;
    public GameObject VNRoot;
    public GameObject GameUI;

    [SerializeField] Sprite ChloeContent;
    [SerializeField] Sprite ChloeCurious;

    public TextArchitect.BuildMethod bm = TextArchitect.BuildMethod.instant;

    public bool VNSceneInProgress;
    private bool startScene;
    private int lineNB;
    public int SceneNb = 1;

    private string[,] currentScene;

    public static VNSceneManager Instance;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            DestroyImmediate(gameObject);
    }
    void Start()
    {
        VNSceneInProgress = false;
        startScene = true;
        VNRoot.SetActive(false);
        lineNB = 0;

        //Set Architect
        ds = DialogueSystem.Instance;
        architect = new TextArchitect(ds._dialogueContainer.dialogueText);
        architect.buildMethod = TextArchitect.BuildMethod.typewriter;
        architect.speed = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        if (VNSceneInProgress && nbScenes >= SceneNb)
        {
            if (startScene)
            {
                PauseGameForScene();
                currentScene = loadScene(SceneNb);
                startScene = false;
                VNRoot.SetActive(true);
                GameUI.SetActive(false);
                lineNB = 0;
                Debug.Log("Scene Lenght is" + currentScene.Length / 3);
                Debug.Log("Beginning: Scene number is" + SceneNb);

                CharacterOne.GetComponent<Image>().sprite = LineCharacter(currentScene[lineNB, 0]);
                ds._dialogueContainer.nameText.text = currentScene[lineNB, 1];
                architect.Build(currentScene[lineNB, 2]);
                lineNB++;
            }
            if (Input.GetKeyDown(KeyCode.Space) && lineNB < currentScene.Length / 3)
            {
                if (architect.isBuilding)
                {
                    if (!architect.fasterText)
                        architect.fasterText = true;
                    else
                        architect.ForceComplete();
                }
                else
                {
                    CharacterOne.GetComponent<Image>().sprite = LineCharacter(currentScene[lineNB, 0]);
                    ds._dialogueContainer.nameText.text = currentScene[lineNB, 1];
                    architect.Build(currentScene[lineNB, 2]);
                    lineNB++;
                }
            }
            else if (Input.GetKeyDown(KeyCode.Space) && lineNB >= currentScene.Length / 3)
            { 
                SceneNb++;
                VNSceneInProgress = false;
                startScene = true;
                VNRoot.SetActive(false);
                GameUI.SetActive(true);
                Debug.Log("End: Scene number is" + SceneNb);
                UnPauseGameForScene();
            }
        }
    }

    private string[,] loadScene(int SC)
    {
        switch(SC)
        {
            case 1:
                return Scene1;
            case 2: 
                return Scene2;
            case 3: 
                return Scene3;
            case 4:
                return Scene4;
            case 5:
                return Scene5;
            case 6:
                return Scene6;
        }

        return null;
    }

    private Sprite LineCharacter(string ch)
    {
        switch(ch)
        {
            case "ChloeContent":
                return ChloeContent;
            case "ChloeCurious":
                return ChloeCurious;
        }

        return null;
    }

    public void PauseGameForScene()
    {
        //Pause the game.
        Time.timeScale = 0;
        GameController.Instance.IsPaused = true;
        Debug.Log("Game Paused for Scene");
    }

    public void UnPauseGameForScene()
    {
        //Pause the game.
        Time.timeScale = 1;
        GameController.Instance.IsPaused = false;
        Debug.Log("Game UnPaused for Scene");
    }
}

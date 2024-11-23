using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor.Experimental.GraphView;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using static WavesManager;

public class VNSceneManager : MonoBehaviour
{
    DialogueSystem ds;
    TextArchitect architect;
    EffectsManager GameMusic;
    public AudioSource NVSoundtrack;
    public AudioSource NVSound;
    public GameObject Chloe;
    public GameObject CharacterTwo;
    public GameObject VNRoot;
    public GameObject GameUI;
    //This game object is the UI element to determine the number of Lore Notes taken
    public GameObject LoreCounterUI;
    //Those game objects is the UI images of the crystals counter
    public GameObject[] CrystalsCounter;

    public TextArchitect.BuildMethod bm = TextArchitect.BuildMethod.instant;

    public bool isTutorial;

    //Those boolean will be use to determine what type of dialogues will be used and which part of the code run (act as blockers)
    public bool VNTutorialInProgress;
    public bool VNLoreInProgess;
    public bool VNSceneInProgress;
    private bool startScene;

    //Those integers will be used to determine the array of dialogues to use (act as counters)
    private int lineNB;
    private int SceneNb = 1;
    private int LoreNb = 1;
    private int TutorialNB = 1;

    //This will be used to determine how many pieces of knowledge have been adquired
    public int loreLearned = 0; 

    //We will use this variable to store the current dialogue array in use. It can be normal Scenes, tutorials or lore scenes
    private string[,] currentScene;

    [SerializeField] Sprite[] ChloeSprites;
    [SerializeField] Sprite[] CharacterTwoSprites;

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
        //To avoid the Visual novel from starting we pre-set those values as false until they are called again
        VNTutorialInProgress = false;
        VNLoreInProgess = false;
        VNSceneInProgress = false;

        //This value is necessary to automatically start the FIRST dialogue without player input
        startScene = true;

        //We deactivate the visual part of the visual novel
        VNRoot.SetActive(false);
        lineNB = 0;

        //This boolean is used to determine if the player is in the tutorial zone. When that is the case, it will be used to deactivate various functions that normally are checked when player is on the main map
        isTutorial = true;

        //Get instance of class where methods StopBackgroundMusic() and ResumeBackgroundMusic() can be found
        GameMusic = EffectsManager.Instance;

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
                Debug.Log("Scene Lenght is" + currentScene.Length / 4);
                Debug.Log("Beginning: Scene number is" + SceneNb);

                Chloe.GetComponent<Image>().sprite = LineCharacter(currentScene[lineNB, 0]);
                ds._dialogueContainer.nameText.text = currentScene[lineNB, 1];
                architect.Build(currentScene[lineNB, 2]);
                lineNB++;
            }
            if (Input.GetKeyDown(KeyCode.Space) && lineNB < currentScene.Length / 4)
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
                    Chloe.GetComponent<Image>().sprite = LineCharacter(currentScene[lineNB, 0]);
                    ds._dialogueContainer.nameText.text = currentScene[lineNB, 1];
                    architect.Build(currentScene[lineNB, 2]);
                    lineNB++;
                }
            }
            else if (Input.GetKeyDown(KeyCode.Space) && lineNB >= currentScene.Length / 4)
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
        else if (VNTutorialInProgress && nbTutorials >= TutorialNB)
        {
            if (startScene)
            {
                PauseGameForScene();
                currentScene = loadTutorial(TutorialNB);
                startScene = false;
                VNRoot.SetActive(true);
                GameUI.SetActive(false);
                lineNB = 0;
                Debug.Log("Tutorial Lenght is" + currentScene.Length / 4);
                Debug.Log("Beginning: Tutorial number is" + TutorialNB);

                ChangeSpriteChloe(lineNB);
                ChangeSpriteCharacterTwo(lineNB);
                ds._dialogueContainer.nameText.text = currentScene[lineNB, 2];
                architect.Build(currentScene[lineNB, 3]);
                lineNB++;
            }
            if (Input.GetKeyDown(KeyCode.Space) && lineNB < currentScene.Length / 4)
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
                    ChangeSpriteChloe(lineNB);
                    ChangeSpriteCharacterTwo(lineNB);
                    ds._dialogueContainer.nameText.text = currentScene[lineNB, 2];
                    architect.Build(currentScene[lineNB, 3]);
                    lineNB++;
                }
            }
            else if (Input.GetKeyDown(KeyCode.Space) && lineNB >= currentScene.Length / 4)
            {
                TutorialNB++;
                VNTutorialInProgress = false;
                startScene = true;
                VNRoot.SetActive(false);
                GameUI.SetActive(true);
                Debug.Log("End: Tutorial number is" + TutorialNB);
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

    private string[,] loadTutorial(int TR)
    {
        switch (TR)
        {
            case 1:
                return Tutorial1;
            case 2:
                return Tutorial2;
            case 3:
                return Tutorial3;
            case 4:
                return Tutorial4;
            case 5:
                return Tutorial5;
            case 6:
                return Tutorial6;
            case 7:
                return Tutorial7;
            case 8:
                return Tutorial8;
            case 9:
                return Tutorial9;
        }

        return null;
    }

    private Sprite LineCharacter(string ch)
    {
        switch(ch)
        {
            case "ChloeAngry":
                return ChloeSprites[0];
            case "ChloeAngryTalking":
                return ChloeSprites[1];
            case "ChloeContent":
                return ChloeSprites[2];
            case "ChloeContentTalking":
                return ChloeSprites[3];
            case "ChloeCry":
                return ChloeSprites[4];
            case "ChloeCryTalking":
                return ChloeSprites[5];
            case "ChloeCurious":
                return ChloeSprites[6];
            case "ChloeFocus":
                return ChloeSprites[7];
            case "ChloeHappy":
                return ChloeSprites[8];
            case "ChloeIndifferent":
                return ChloeSprites[9];
            case "ChloeIndifferentTalking":
                return ChloeSprites[10];
            case "ChloeSad":
                return ChloeSprites[11];
            case "ChloeSadTalking":
                return ChloeSprites[12];
            case "ChloeSmug":
                return ChloeSprites[13];
            case "ChloeTears":
                return ChloeSprites[14];
            case "ChloeTearsTalking":
                return ChloeSprites[15];
            case "ChloeTearsHappy":
                return ChloeSprites[16];
            case "ChloeAnnoyed":
                return ChloeSprites[17];
            case "ChloeAnnoyedTalking":
                return ChloeSprites[18];
            case "RayneAngry": //Rayna's Sprites start here
                return CharacterTwoSprites[0];
            case "RayneAngryTalking":
                return CharacterTwoSprites[1];
            case "RayneHappy":
                return CharacterTwoSprites[2];
            case "RayneHappyTalking":
                return CharacterTwoSprites[3];
            case "RayneIndifferent":
                return CharacterTwoSprites[4];
            case "RayneIndifferentTalking":
                return CharacterTwoSprites[5];
            case "RayneSad":
                return CharacterTwoSprites[6];
            case "RayneSadTalking":
                return CharacterTwoSprites[7];
            case "RayneSmirk":
                return CharacterTwoSprites[8];
            case "RayneSmirkTalking":
                return CharacterTwoSprites[9];
            case "RayneThinking":
                return CharacterTwoSprites[10];
            case "RayneGemForm":
                return CharacterTwoSprites[11];
        }

        return null;
    }

    private void ChangeSpriteChloe(int ln)
    {
        if (currentScene[ln, 0] == "")
            Chloe.SetActive(false);
        else
        {
            Chloe.SetActive(true);
            Chloe.GetComponent<Image>().sprite = LineCharacter(currentScene[ln, 0]);
        }
    }
    private void ChangeSpriteCharacterTwo(int ln)
    {
        if (currentScene[ln, 1] == "")
            CharacterTwo.SetActive(false);
        else
        {
            CharacterTwo.SetActive(true);
            CharacterTwo.GetComponent<Image>().sprite = LineCharacter(currentScene[ln, 1]);
        }
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


    public void StartStoryScene()
    {
        VNSceneInProgress = true;
    }

    public void StartTutorialScene()
    {
        VNTutorialInProgress = true;
    }

    public void StartLoreScene()
    {
        VNLoreInProgess = true;
    }


    #region Scenes
    private int nbScenes = 6;

    /// Here, we have lines that are made of 4 elements. 
    /// Element 1: Determines WHICH character sprite is on the object or IF character object is active for FIRST character
    /// Element 2: Determines WHICH character sprite is on the object or IF character object is active for SECOND character
    /// Element 3: The name of the current speaker
    /// Element 4: Dialogue
    string[,] Scene1 = new string[5, 4]
    {
        { "ChloeContent", "", "Chloe", "Dialogue 1 of Scene 1" } ,
        { "ChloeContent", "", "Chloe", "Dialogue 2 of Scene 1" },
        { "ChloeCurious", "", "Chloe", "Dialogue 3 of Scene 1" },
        { "ChloeContent", "", "Chloe", "Dialogue 4 of Scene 1" },
        { "ChloeCurious", "", "Chloe", "Dialogue 5 of Scene 1" }
    };

    string[,] Scene2 = new string[5, 4]
    {
        { "ChloeContent", "", "Chloe", "Dialogue 1 of Scene 2" } ,
        { "ChloeContent", "", "Chloe", "Dialogue 2 of Scene 2" },
        { "ChloeCurious", "", "Chloe", "Dialogue 3 of Scene 2" },
        { "ChloeContent", "", "Chloe", "Dialogue 4 of Scene 2" },
        { "ChloeCurious", "", "Chloe", "Dialogue 5 of Scene 2" }
    };

    string[,] Scene3 = new string[5, 4]
    {
        { "ChloeContent", "", "Chloe", "Dialogue 1 of Scene 3" } ,
        { "ChloeContent", "", "Chloe", "Dialogue 2 of Scene 3" },
        { "ChloeCurious", "", "Chloe", "Dialogue 3 of Scene 3" },
        { "ChloeContent", "", "Chloe", "Dialogue 4 of Scene 3" },
        { "ChloeCurious", "", "Chloe", "Dialogue 5 of Scene 3" }
    };

    string[,] Scene4 = new string[5, 4]
    {
        { "ChloeContent", "", "Chloe", "Dialogue 1 of Scene 4" } ,
        { "ChloeContent", "", "Chloe", "Dialogue 2 of Scene 4" },
        { "ChloeCurious", "", "Chloe", "Dialogue 3 of Scene 4" },
        { "ChloeContent", "", "Chloe", "Dialogue 4 of Scene 4" },
        { "ChloeCurious", "", "Chloe", "Dialogue 5 of Scene 4" }
    };

    string[,] Scene5 = new string[5, 4]
    {
        { "ChloeContent", "", "Chloe", "Dialogue 1 of Scene 5" } ,
        { "ChloeContent", "", "Chloe", "Dialogue 2 of Scene 5" },
        { "ChloeCurious", "", "Chloe", "Dialogue 3 of Scene 5" },
        { "ChloeContent", "", "Chloe", "Dialogue 4 of Scene 5" },
        { "ChloeCurious", "", "Chloe", "Dialogue 5 of Scene 5" }
    };

    string[,] Scene6 = new string[5, 4]
    {
        { "ChloeContent", "", "Chloe", "Dialogue 1 of Scene 6" } ,
        { "ChloeContent", "", "Chloe", "Dialogue 2 of Scene 6" },
        { "ChloeCurious", "", "Chloe", "Dialogue 3 of Scene 6" },
        { "ChloeContent", "", "Chloe", "Dialogue 4 of Scene 6" },
        { "ChloeCurious", "", "Chloe", "Dialogue 5 of Scene 6" }
    };
    #endregion

    #region Tutorials

    private int nbTutorials = 9;

    string[,] Tutorial1 = new string[11, 4]
    {
        {"ChloeAngry", "", "Chloe", "Ugghhhh……… What…. Where…."},
        { "ChloeAngry", "", "????", "About time you woke up dear~"},
        {"ChloeCurious", "", "Chloe", "!!!"},
        {"ChloeIndifferentTalking", "", "Chloe", "H-How is there?! I might be a little and a girl, but I know how to shoot magic!"},
        {"ChloeIndifferent", "", "????", "Relax, relax Chloe~ I mean no harm. You just woke up, so don’t over do yourself despite not been a little girl anymore. As for who I am...."},
        {"ChloeCurious", "RayneGemForm", "????", "Morning~!"},
        {"ChloeCurious", "RayneGemForm", "Chloe", "....."},
        {"ChloeCurious", "RayneGemForm", "????", "....."},
        {"ChloeIndifferentTalking", "RayneGemForm", "Chloe", "Maybe if I go back to sleep, I should wake up soon…."},
        {"ChloeFocus", "RayneGemForm", "????", "Waitwaitwait dear! I know it is weird waking up in a creepy place like this with a floating perfect blue gorgeous gem like me that talks~ BUT…. You need to listen to me Chloe. You might not understand what is happening right now, but know that you are in danger"},
        {"ChloeCurious", "RayneGemForm", "Tutorial", "(To move the character, use WASD. W to go up, A to go left, S to go down, and D to go right. Move and navigate through the pillar to the next room)"}
    };

    string[,] Tutorial2 = new string[12, 4]
    {
        { "ChloeIndifferentTalking", "RayneGemForm", "Chloe", "Stop! I will not continue to follow you until you tell me more. What do you mean by being in danger? And if this is not a dream, where I am, how do you know me, and how it is possible that I got taller?" },
        { "ChloeIndifferent", "RayneGemForm", "????", "Well…. It is complicated. But lets beginning by telling you about your new status as a WOMAN and… anatomy. Here, check your reflection on me" },
        { "ChloeIndifferentTalking", "RayneGemForm", "Chloe", "What do you mea- AAHH monster!! Wait…. Is that me?! I am…. An adult? And I am blue?!! E-Eh…? Is that a tail….?!" },
        { "ChloeIndifferentTalking", "RayneGemForm", "????", "Chloe, tell me, what do you remember?" },
        { "ChloeAngryTalking", "RayneGemForm", "Chloe", "I ahh…. Uh. This is weird. I DO remember my name and the fact that I was a human girl and not…. This. But I cannot think of anything else. It is just…. Blurry when I think about it" },
        { "ChloeCurious", "RayneGemForm", "????", "So amnesia…. No. I think it is simply your memories been back under the surface of your mind. This might be a consequence of been trapped and connected to the hivemind" },
        { "ChloeCurious", "RayneGemForm", "Chloe", "Hivemind?" },
        { "ChloeCurious", "RayneGemForm", "????", "Yes. A collective of minds, consciences, life essences or souls all connected into one. All the beings and objects inside this place are connected to it by the natural and corrupted mana in the air. You were trapped there for a long time. Your body, the one you are using right now, has slowly age overtime and changed with the corruption" },
        { "ChloeIndifferentTalking", "RayneGemForm", "Chloe", "I… I see. Yeah, I can feel it now that I am thinking and focusing about it. The air is saturated with magic. It feels…. Dirty. I should not be able to breath or even thinking right now due to how corrupted it is but… Is it because of my body? And how did I get out?" },
        { "ChloeIndifferent", "RayneGemForm", "????", "Thanks to your family heirloom, the Azur Core. It is a necklace with a blue gem basically. You found it and it acted as an anchor to pull you away from the hive. The necklace was destroyed in the process, but the gem remains… which is me~!" },
        { "ChloeIndifferent", "RayneGemForm", "????", "Anyway… although you are not longer human, you still have access to your magic, and you are stronger and faster than before! See those big pots blocking the way? Try to destroy them" },
        { "ChloeIndifferent", "RayneGemForm", "Tutorial", "(To fire your weapon, point the mouse in the direction where you want to shoot and press Left Click to fire a bullet)" }
    };

    string[,] Tutorial3 = new string[9, 4]
    {
        { "ChloeIndifferentTalking", "RayneGemForm", "Chloe", "So… Basically, you are a gem that is gained sentience after pulling me out?" },
        { "ChloeCurious", "RayneGemForm", "????", "Nonono… I am a living being! Or…. I was long ago, but my soul is intact and inside the Azur Core. Soooo… I am technically alive~!" },
        { "ChloeIndifferentTalking", "RayneGemForm", "Chloe", "I see. Then who are you?" },
        { "ChloeCurious", "RayneGemForm", "????", "Nice you ask dear~! I am ******************, it is good to ***** you ****** Chloe~!" },
        { "ChloeIndifferentTalking", "RayneGemForm", "Chloe", "Uhhh…. Sorry, but your words were…. Distorted? Censored? Anyway, I could not understand you" },
        { "ChloeCurious", "RayneGemForm", "????", "Damn… Not only your memories are blackened, but it seems the hive still as influence on you to the point of blocking any information that might get them surface." },
        { "ChloeCurious", "RayneGemForm", "????", "Maybe the connection is now only starting to fade? I am not sure… OH! A chest~! Go and open it, we might find something useful that can help you. This place is certainly full of fancy stuff~" },
        { "ChloeIndifferent", "RayneGemForm", "Tutorial", "(To interact with chests and other interactable objects, you need to approach them until an icon appears. Using your mouse, Left Click to start the interaction)" },
        { "ChloeIndifferent", "RayneGemForm", "Tutorial", "(There are five types of chests. From those, copper and iron chests can be open without the need of a key. Small and large gold chests as well as blue-platinum chests require an unequiped key in the inventory to open)" }
    };

    string[,] Tutorial4 = new string[6, 4]
    {
        { "ChloeAnnoyed", "RayneGemForm", "????", "Nothing eh…? Well, it can’t be helped~! Until you start getting your memories back, it is what it is" },
        { "ChloeAnnoyedTalking", "RayneGemForm", "Chloe", "Well, is not that wonderful… I guess my puppet master could not leave me alone. Anyway…. If I cannot get your true name, then how should I call you" },
        { "ChloeIndifferent", "RayneGemForm", "Azure", "Well dear, you can call me Azure~!" },
        { "ChloeContentTalking", "RayneGemForm", "Chloe", "Fitting. Nice to meet you, Azure. So, what now? How do we get out of here? Do you know the way?" },
        { "ChloeIndifferent", "RayneGemForm", "Azure", "I DO know the way~ Our objective is to react the Crystal Nexus in this floor. What is a Crystal Nexus you ask? Well… You will see. It is very eye-catching, so you will recognize it at first glance~. But before we continue, we must surpass any obstacle in our way… Starting with that pillar!" },
        { "ChloeIndifferent", "RayneGemForm", "Tutorial", "(Some breakable objects might take multiple attacks to destroy. Attack and destroy the pillar blocking the way)" }
    };

    string[,] Tutorial5 = new string[5, 4]
    {
        { "ChloeIndifferentTalking", "RayneGemForm", "Chloe", "Look, there is a note there! Maybe somebody left it here" },
        { "ChloeCurious", "RayneGemForm", "Azure", "Perhaps… But I am quite sure that person is already dead… Maybe he or she have been dead for a long time. This place is very old, and it has a lot of history. History that I don’t know. I don’t even know how this place came to be. The only important matter is to survive and get out of here" },
        { "ChloeContentTalking", "RayneGemForm", "Chloe", "I see… Maybe there is nobody alive down here that is not part of the hive to tell us… But things like notes, documents and records might have remained intact! They tell a story… So, I might be able to recover my memories if I learn more about this place!" },
        { "ChloeHappy", "RayneGemForm", "Azure", "That is… Actually, a good idea! Yeah, it might work. By gaining enough information and knowledge, you might be able to pull back your missing memories! Go on, lets see what it says~" },
        { "ChloeHappy", "RayneGemForm", "Tutorial", "(Approach and interact with the paper. There multiple paper record around the map like this one. To help Chloe regain more of her memories, try to explore the map in search of them as a secondary objective)" }
    };

    string[,] Tutorial6 = new string[5, 4]
    {
        { "ChloeCurious", "RayneGemForm", "Azure", "Stop!" },
        { "ChloeIndifferentTalking", "RayneGemForm", "Chloe", "What? What is it?" },
        { "ChloeIndifferent", "RayneGemForm", "Azure", "There are traps ahead. It seems the hive… Labyssal is starting to recover its senses. Don’t worry those ones are harmless, but I fear there might be others ahead less adventure friendly" },
        { "ChloeIndifferentTalking", "RayneGemForm", "Chloe", "Got it" },
        { "ChloeIndifferent", "RayneGemForm", "Tutorial", "(There are multiple traps in the maze, but they normally fall in 3 categories: Traps that give a stat penalty, traps that do instant damage, and traps that do continuous damage. The one in front of Chloe reduces her speed when walking through it)" }
    };

    string[,] Tutorial7 = new string[2, 4]
    {
        { "ChloeIndifferent", "RayneGemForm", "Azure", "Careful there! Those holes look harmless, but they are spikes down there. You should dash to pass them fast Chloe" },
        { "ChloeIndifferent", "RayneGemForm", "Tutorial", "(To avoid traps like the spikes in front, use Dash to go fast through them. Use the Space key to Dash in the direction where you are moving)" }
    };

    string[,] Tutorial8 = new string[2, 4]
    {
        { "ChloeAnnoyed", "RayneGemForm", "Azure", "Lava… Be careful Chloe. Try to not stay long close to it or you might burn" },
        { "ChloeAnnoyed", "RayneGemForm", "Tutorial", "(If Chloe stays too long on top of lava tiles, she will start receiving continuous damage and receive the burning status that will temporarily continue to damage her even after exiting the tile)" }
    };

    string[,] Tutorial9 = new string[19, 4]
    {
        { "ChloeIndifferentTalking", "RayneGemForm", "Chloe", "So that is the Nexus… It is beautiful" },//1
        { "ChloeIndifferent", "RayneGemForm", "Azure", "It is. But it was not the case a few minutes ago. The crystals you see were purified after pulling you out of the hivemind. Before that, they were corrupted, thus making the Nexus unable to work. But now it does, and it is the key to getting out of here!" },//2
        { "ChloeIndifferentTalking", "RayneGemForm", "Chloe", "But WHAT does the nexus do? You have not told me, except that it can help us getting out of here" },//3
        { "ChloeIndifferent", "RayneGemForm", "Azure", "Right! So… The nexus is basically a device to manipulate space. It will send the user towards another location. In this case, this Nexus will send us up closer to the surface. However..." },//4
        { "ChloeAnnoyedTalking", "RayneGemForm", "Chloe", "I am not going to like the next part, do I...?" },//5
        { "ChloeIndifferent", "RayneGemForm", "Azure", "Nop. This baby here will send us up… Inside the maze made by Labyssal if that note is to be believed. In order to get out completely, we need to find the OTHER Nexus on the next floor and activate it. " },//6
        { "ChloeAnnoyedTalking", "RayneGemForm", "Chloe", "And to do that, we need to purify the Energy Crystals that feed power to the Nexus… Crystals that might be far one from the other…. Wonderful" },//7
        { "ChloeAnnoyed", "RayneGemForm", "Azure", "Don’t be grumpy dear~ I will be there with you all the way!" },//8
        { "ChloeAnnoyedTalking", "RayneGemForm", "Chloe", "You are a floating rock, what can you do?!" },//9
        { "ChloeAnnoyed", "RayneGemForm", "Azure", "You underestimate me hehe…. I was not able to do this due to the lack of magical energy, but now with a nexus to give it to me…. Watch!" },//10
        { "ChloeCurious", "", "Azure", "Transfooooormmmm…… AAH!!! " },//11
        { "ChloeIndifferentTalking", "", "Chloe", "Wh…What the hell?!" },//12
        { "ChloeCurious", "RayneHappyTalking", "Azure", "TADA~!! Back and good as new~!" },//13
        { "ChloeIndifferentTalking", "RayneSmirk", "Chloe", "You…. You were a rock!! How did you get back your body?!" },//14
        { "ChloeIndifferent", "RayneHappyTalking", "Azure", "I didn’t. This is a body made of hard light using mana. I am just a projection so… I am technically still a rock~! But I will more comfortable like this" },//15
        { "ChloeIndifferentTalking", "RayneSmirk", "Chloe", "I see… And your clothes?" },//16
        { "ChloeCurious", "RayneIndifferentTalking", "Azure", "Still working on it. Anyway, that is not important! Are you ready or not?" },//17
        { "ChloeIndifferentTalking", "RayneSmirk", "Chloe", "Yeah yeah… There is not other choice, but sure I am" },//18
        { "ChloeIndifferent", "RayneSmirk", "Tutorial", "(When you are ready, approach the Nexus and interact with it. You will be teleported towards the main area, where you must find and purify the energy crystals in the maze. Once that is done you can exit the maze via the local Nexus. As a secondary objective, you can try to collect all the collectable record notes in the map. They will influence the ending of the game)" }//19
    };

    #endregion
}

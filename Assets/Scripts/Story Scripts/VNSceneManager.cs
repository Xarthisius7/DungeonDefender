using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    private bool isRayne;

    //Those integers will be used to determine the array of dialogues to use (act as counters)
    private int lineNB;
    public int SceneNb = 3;
    public int LoreNb = 1;
    public int TutorialNB = 1;

    //This will be used to determine how many pieces of knowledge have been adquired
    public int loreLearned = 0;

    public TMP_Text loreLearnedCounterUI;

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
        isRayne = false;

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

        loreLearnedCounterUI.text = loreLearned.ToString() + " / " + nbLores;
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
                Debug.Log("Scene Lenght is " + currentScene.Length / 4);
                Debug.Log("Beginning: Scene number is " + SceneNb);

                ChangeSpriteChloe(lineNB);
                ChangeSpriteCharacterTwo(lineNB);
                ds._dialogueContainer.nameText.text = NameRayneforStory(currentScene[lineNB, 2]);
                architect.Build(currentScene[lineNB, 3]);
                lineNB++;
            }

            if (Input.GetKeyDown(KeyCode.Tab))
                EndCurrentScene();

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
                    ds._dialogueContainer.nameText.text = NameRayneforStory(currentScene[lineNB, 2]);
                    architect.Build(currentScene[lineNB, 3]);
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
                Debug.Log("End: Scene number is " + SceneNb);
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
                Debug.Log("Tutorial Lenght is " + currentScene.Length / 4);
                Debug.Log("Beginning: Tutorial number is " + TutorialNB);

                ChangeSpriteChloe(lineNB);
                ChangeSpriteCharacterTwo(lineNB);
                ds._dialogueContainer.nameText.text = currentScene[lineNB, 2];
                architect.Build(currentScene[lineNB, 3]);
                lineNB++;
            }

            if (Input.GetKeyDown(KeyCode.Tab))
                EndCurrentScene();

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
                Debug.Log("End: Tutorial number is " + TutorialNB);
                UnPauseGameForScene();
            }
        }
        else if (VNLoreInProgess && nbLores >= LoreNb)
        {
            if (startScene)
            {
                PauseGameForScene();
                currentScene = loadLore(LoreNb);
                startScene = false;
                VNRoot.SetActive(true);
                GameUI.SetActive(false);
                lineNB = 0;
                Debug.Log("Lore Lenght is " + currentScene.Length / 4);
                Debug.Log("Beginning: Lore number is " + LoreNb);

                ChangeSpriteChloe(lineNB);
                ChangeSpriteCharacterTwo(lineNB);
                ds._dialogueContainer.nameText.text = currentScene[lineNB, 2];
                architect.Build(currentScene[lineNB, 3]);
                lineNB++;
            }

            if (Input.GetKeyDown(KeyCode.Tab))
                EndCurrentScene();

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
                LoreNb++;
                loreLearned++;
                loreLearnedCounterUI.text = loreLearned.ToString() + " / " + nbLores;
                VNLoreInProgess = false;
                startScene = true;
                VNRoot.SetActive(false);
                GameUI.SetActive(true);
                Debug.Log("End: Lore number is " + LoreNb);
                UnPauseGameForScene();
            }
        }
    }

    private string[,] loadScene(int SC)
    {
        switch (SC)
        {
            case 1:
                return Scene1;
            case 2:
                return Scene2;
            case 3:
                return Scene3;
            case 4:
                if (loreLearned < 8)
                    return Scene4A;
                else if (loreLearned >= 8 && loreLearned < 19)
                    return Scene4B;
                else
                    return Scene4C;
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

    private string[,] loadLore(int LR)
    {
        switch (LR)
        {
            case 1:
                return Lore1;
            case 2:
                return Lore2;
            case 3:
                return Lore3;
            case 4:
                return Lore4;
            case 5:
                return Lore5;
            case 6:
                return Lore6;
            case 7:
                return Lore7;
            case 8:
                return Lore8;
            case 9:
                return Lore9;
            case 10:
                return Lore10;
            case 11:
                return Lore11;
            case 12:
                return Lore12;
            case 13:
                return Lore13;
            case 14:
                return Lore14;
            case 15:
                return Lore15;
            case 16:
                return Lore16;
            case 17:
                return Lore17;
            case 18:
                return Lore18;
            case 19:
                return Lore19;
        }

        return null;
    }

    private Sprite LineCharacter(string ch)
    {
        switch (ch)
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

    private string NameRayneforStory(string nm)
    {
        if (nm == "Azure" && isRayne)
            return "Rayne";
        else
            return nm;
    }

    private void EndCurrentScene()
    {
        if (VNSceneInProgress)
        {
            SceneNb++;
            VNSceneInProgress = false;
            startScene = true;
            VNRoot.SetActive(false);
            GameUI.SetActive(true);
            Debug.Log("End: Scene number is " + SceneNb);
            UnPauseGameForScene();
        }
        else if (VNTutorialInProgress)
        {
            TutorialNB++;
            VNTutorialInProgress = false;
            startScene = true;
            VNRoot.SetActive(false);
            GameUI.SetActive(true);
            Debug.Log("End: Tutorial number is " + TutorialNB);
            UnPauseGameForScene();
        }
        else if (VNLoreInProgess)
        {
            LoreNb++;
            loreLearned++;
            loreLearnedCounterUI.text = loreLearned.ToString() + " / " + nbLores;
            VNLoreInProgess = false;
            startScene = true;
            VNRoot.SetActive(false);
            GameUI.SetActive(true);
            Debug.Log("End: Lore number is " + LoreNb);
            UnPauseGameForScene();
        }
    }

    public int getSceneNB()
    {
        return SceneNb;
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
    string[,] Scene1 = new string[8, 4]
    {
        //1
        {"ChloeIndifferentTalking", "RayneSmirk", "Chloe", "Well... that was exiting for a first one..."},
        //2
        {"ChloeIndifferent", "RayneHappyTalking", "Azure", "And it was the easiest one. Don't forget there are still 2 more to go~"},
        //3
        {"ChloeAnnoyedTalking", "RayneHappy", "Chloe", "Right... You seem happy, yet I am the one doing the heavy lifting"},
        //4
        {"ChloeAnnoyed", "RayneHappyTalking", "Azure", "Hehe... Don't be so grumpy dear~ You might be the one doing most of the fighting, but I am the one purifying the crystals~"},
        //5
        {"ChloeIndifferentTalking", "RayneSmirk", "Chloe", "I get that. So? How do we know they are not going to be corrupted again after leaving them alone?"},
        //6
        {"ChloeCurious", "RayneHappyTalking", "Azure", "I applied a layer of solid mana around it. It will help to filtrate and keep out the corruption in the air. I will admit, it is not perfect, but it will give us more than the entire day to clean up the others"},
        //7
        {"ChloeContentTalking", "RayneHappy", "Chloe", "hmmmm~ That is smart. Well, who would have guessed, you competent after all!" },
        //8
        {"ChloeHappy", "RayneAngryTalking", "Azure", "Ooe..." }
    };

    string[,] Scene2 = new string[5, 4]
    {
        //1
        {"ChloeAnnoyed", "RayneIndifferentTalking", "Azure", "It is getting harder and harder. I think the maze is now actively responding to our threat... Or is simply irritated we are such noisy brats"},
        //2
        {"ChloeAnnoyedTalking", "RayneIndifferent", "Chloe", "And guess who is the one that has to bear the consequences...."},
        //3
        {"ChloeAnnoyed", "RayneHappyTalking", "Azure", "Well, it cannot be helped, you are the demoness and I am the cute and fagile rock after all~!"},
        //4
        {"ChloeContentTalking", "RayneAngry", "Chloe", "Sure... if you replace the cute with the old"},
        //5
        {"ChloeContent", "RayneIndifferentTalking", "Chloe", "Brat..."}
    };

    string[,] Scene3 = new string[5, 4]
    {
        //1
        {"ChloeIndifferentTalking", "RayneIndifferent", "Chloe", "That was the last one, right?"},
        //2
        {"ChloeSad", "RayneIndifferentTalking", "Azure", "Yep. Which means it is now time to get out of here... You ok Chloe?"},
        //3
        {"ChloeSadTalking", "RayneIndifferent", "Chloe", "Yeah, I am just... Thinking about the memories that I have of this place... How it seems familiar, yet foreigner now that I am leaving it behind. It was... home for a while"},
        //4
        {"ChloeSad", "RayneSadTalking", "Azure", "I see... Ok no, I actually don't. I was trapped inside the Core for so long that my ultimate goal has been always on getting out so... I actually don't know how you feel, but I understand wha you mean"},
        //5
        {"ChloeContentTalking", "RayneSmirk", "Chloe", "Thanks I guess. But yeah, I agree. It is time to leave. Lets go find that Nexus!"}
    };

    string[,] Scene4A = new string[6, 4]
    {
        //1
        {"ChloeIndifferent", "RayneSmirkTalking", "Azure", "There is the Nexus!!! Ah... The smell of freedom~ I can already taste it"},
        //2
        {"ChloeContentTalking", "RayneSmirk", "Chloe", "Personally, I am more interested on seen the sun after so long. Although just to get to see the world ... And see how it is like, would be interesting indeed"},
        //3
        {"ChloeSadTalking", "RayneIndifferent", "Chloe", "But... I am still wondering... about us, you know? Who we were before ending here. What if we had family? I guess they most have died by now from old age, I am just wondering"},
        //4
        {"ChloeSad", "RayneSadTalking", "Azure", "I guess... you are right. In the end, we never discover who we were. But is that such a bad thing? So much time has pass... That regardless if we had lives before this, they are not here anymore. Which means a new start..."},
        //5
        {"ChloeContent", "RayneHappyTalking", "Azure", "An adventure, travelling around the world... you and me, together!"},
        //6
        {"ChloeContentTalking", "RayneSmirk", "Chloe", "Yeah... Together!"}
    };

    string[,] Scene4B = new string[8, 4]
    {
        //1
        {"ChloeAnnoyed", "RayneSmirkTalking", "Azure", "Fascinating... So this is the spatial disruptor at its full glory! Been as sensitive as I am to magic, I can see the pulses of mana. It is like the professor said! The receptors receive, calculate, and space is swapped!"},
        //2
        {"ChloeAnnoyedTalking", "RayneSmirk", "Chloe", "You know... I get it. You are a fan now, and it seems you found a hobby in science. There are a lot of cool stuff here, you can stay will I go up to claim my place under the sun"},
        //3
        {"ChloeAnnoyed", "RayneHappyTalking", "Azure", "Hehe, no way I will let you go away and leave me in the dust~ I also have plans in the surface. Like making myself a golem to use as a body! We got a lot of notes from Professor Jones, and there are some interesting ideas he mentions during other entries. Also, I am not sure that our dear homicidal maze will allow me to play with its toys~"},
        //4
        {"ChloeIndifferent", "RayneIndifferentTalking", "Azure", "But... Although we learned that there is a world up there, there are also dangers. And considering where we come from... people will not look kind to us... to you Chloe. The fact is that you are not human, and people will not like that, so..."},
        //5
        {"ChloeAngryTalking", "RayneIndifferent", "Chloe", "So...?"},
        //6
        {"ChloeCurious", "RayneHappyTalking", "Azure", "I will get more reliable and strong... To protect you"},
        //7
        {"ChloeContentTalking", "RayneSmirk", "Chloe", "Hehehehe...! Ironic, since I was the one protecting you all along until now. But... yeah. That makes me happy, to have you at my side"},
        //8
        {"ChloeHappy", "RayneHappyTalking", "Azure", "Yes, to your side now and always"}
    };

    string[,] Scene4C = new string[18, 4]
    {
        //1
        {"ChloeSad", "RayneSadTalking", "Rayne", "Chloe... darling... We have to go."},
        //2
        {"ChloeSadTalking", "RayneSad", "Chloe", "I know. It is just that... even now, I can feel their connection towards me. I can feel some of the more intelligent ones begging in their own way for their queen, for Labyssal to not leave."},
        //3
        {"ChloeSadTalking", "RayneSad", "Chloe", "And before you ask, having a connection is not the same as been on synch with the collective. I am still myself, I am still Chloe. But... I know that once I, their queen, am gone, this place will be back as it was before. A collective without ambition nor aim. Only following my last orders on defending this place to the death. Which is why I am sad, yet relieve that our home will continue to exist for the next centuries"},
        //4
        {"ChloeSad", "RayneIndifferentTalking", "Rayne", "But that is not the only thing, it is Chloe? You don't really fear leaving because of what you will leave behind.... but for what you might find out"},
        //5
        {"ChloeSadTalking", "RayneIndifferent", "Chloe", "You are right mom. Like I told you, part of me was satisfied that I got to destroy the empire, but another recognizes the pain that I brought others in doing so. That makes me wonder if they will look at me like the crusaders did back then? I destroyed what I hated, yet I ended becoming like it, thus bringing ruin to people."},
        //6
        {"ChloeSad", "RayneIndifferentTalking", "Rayne", "But those, The Black Wave and the crusades, were centuries ago. History might remember, but humanity as a whole is bad at doing it..."},
        //7
        {"ChloeContent", "RayneHappyTalking", "Rayne", "Else, the fools would not have started a Second Continental War."},
        //8
        {"ChloeContentTalking", "RayneIndifferent", "Chloe", "Yes, I guess you are right. But... Then what should I do? We don't have a home anymore. Mages might still be discriminated to this day... Why go out?"},
        //9
        {"ChloeCurious", "RayneIndifferentTalking", "Rayne", "Honestly... Despite been a cute and baddy maze, my daughter can certainly be clueless...? To answer your question, we will do what YOU wanted to do if you ever had escaped the mines... We will restore the Azulheard clan, and get back what we lost"},
        //10
        {"ChloeIndifferentTalking", "RayneIndifferent", "Chloe", "I guess... that we would both like that. But, mom, what if someone tries to stop us? What will people think about my appearance? What if we get attack?"},
        //11
        {"ChloeCurious", "RayneIndifferentTalking", "Rayne", "<i> Sigh </i>... Simple. On my part, I will use my now love for science and professor Jones notes to make myself a body, but you Chloe... You should NOT have any problem at all"},
        //12
        {"ChloeIndifferentTalking", "RayneIndifferent", "Chloe", "What? Why?"},
        //13
        {"ChloeCurious", "RayneHappyTalking", "Rayne", "Who strong you think you are right now silly~? Or are you telling me that you lost all your power simply because you got desynchronized from the maze?"},
        //14
        {"ChloeIndifferentTalking", "RayneIndifferent", "Chloe", "Oh... Not actually. As Labyssal, my soul gives my body the ability to interact intimately with mana. If I wanted to, I could... take control of an area and make it my own. But why are you saying that? I though..."},
        //15
        {"ChloeCurious", "RayneHappyTalking", "Rayne", "What I would keep a leash on you for eternity? Eheheh... goodness no! Look Chloe... I am your mother. Not a hero that seeks to bring balance to the world."},
        //16
        {"ChloeCry", "RayneIndifferentTalking", "Rayne", "I am... disappointed and it makes me sad to think you did what you did. But I don't plan on chaining you nor pushing you in a quest for redemption. I want to set you free... And make sure you are happy"},
        //17
        {"ChloeHappy", "RayneHappy", "Chloe", "<i> sniff </i>... Love you mom~!"},
        //18
        {"ChloeHappy", "RayneHappyTalking", "Rayne", "Love you too sweetheart. Now... lets go out and reclaim our place under the sun..."}
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

    string[,] Tutorial2 = new string[14, 4]
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
        { "ChloeIndifferent", "RayneGemForm", "????", "Thanks to the Azur Core. It is a necklace with a blue gem basically. You found it and it acted as an anchor to pull you away from the hive. The necklace was destroyed in the process, but the gem remains… which is me~!" },
        { "ChloeIndifferentTalking", "RayneGemForm", "Chloe", "And you know my name because..." },
        { "ChloeIndifferent", "RayneGemForm", "????", "Because after I pull you out of the hivemind, I got a few of your memories... Or at least the ones you remember right now. And that is how you know your name and about your situation" },
        { "ChloeIndifferent", "RayneGemForm", "????", "Anyway… although you are not longer human, you still have access to your magic, and you are stronger and faster than before! See those big pots blocking the way? Try to destroy them" },
        { "ChloeIndifferent", "RayneGemForm", "Tutorial", "(To fire your weapon, point the mouse in the direction where you want to shoot and press Left Click to fire a bullet)" }
    };

    string[,] Tutorial3 = new string[12, 4]
    {
        { "ChloeIndifferentTalking", "RayneGemForm", "Chloe", "So… Basically, you are a gem that is gained sentience after pulling me out?" },
        { "ChloeCurious", "RayneGemForm", "????", "Nonono… I am a living being! Or…. I was long ago, but my soul is intact and inside the Azur Core. I was inside the Core for a long time. Somewhat aware of my surroundings, but unable to do anything... AND BORED OUT OF MY MIND! That is, until you pick me. Soooo… I am technically alive~!" },
        { "ChloeIndifferentTalking", "RayneGemForm", "Chloe", "And then you decided to pull me out when my body touch you" },
        { "ChloeAnnoyed", "RayneGemForm", "????", "uuuuhhhh... not...exactly. I actually tried to take control of your body since it was basically empty of a soul. But, to my surprise, this had the effect of pulling... Oooh don't look at me like that! I confese, it was an accident and did not do it out of the goodness of my heart, but it is a win-win at the end~" },
        { "ChloeAnnoyedTalking", "RayneGemForm", "Chloe", "I see. Good to know my return was an accident. So... who are you anyway, oh my selfless savior?" },
        { "ChloeAnnoyed", "RayneGemForm", "????", "Geeh... You are not going to let that go, don't you? Anyway, it is funny, but like you, I don't really know who I am. I have been traped here so long that both time and erosion by corruption has made most of my memories fade" },
        {"ChloeCurious", "RayneGemForm", "????", "But... contrary to you, I do not even remember my name. Maybe I will with time..."},
        { "ChloeSadTalking", "RayneGemForm", "Chloe", "Uhhh…. Sorry, for asking then. I guess we are in the same situation then. Maybe both our memories will come as time goes. Also, how come I am the first of everybody to touch you and interact with you" },
        { "ChloeCurious", "RayneGemForm", "????", "Well, that is the thing. I don't know how YOU got to see me. You see, the Azur Core has an enchantment. A powerful one. It basically not only makes invisible to everybody, but it also creates a zone around it where sentient beings will unconciously avoid it. That is why the hivemind was unable to see me. Which makes it more surprising that you did." },
        { "ChloeCurious", "RayneGemForm", "????", "Maybe your body was naturally more sensible to magic, or the enchantment's effects were starting to fade... I am not sure… OH! A chest~! Go and open it, we might find something useful that can help us. This place is certainly full of fancy stuff~" },
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
        { "ChloeContentTalking", "RayneGemForm", "Chloe", "I see… Maybe there is nobody alive down here that is not part of the hive to tell us… But things like notes, documents and records might have remained intact! They tell a story… So, I might be able to recover my or your memories if we learn more about this place!" },
        { "ChloeHappy", "RayneGemForm", "Azure", "That is… Actually, a good idea! Yeah, it might work. By gaining enough information and knowledge, we might be able to get our missing memories back! Go on, lets see what it says~" },
        { "ChloeHappy", "RayneGemForm", "Tutorial", "(Approach and interact with the paper. There multiple paper record around the map like this one. To help Chloe and Azure regain more of their memories, try to explore the map in search of them as a secondary objective)" }
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
        { "ChloeIndifferentTalking", "RayneGemForm", "Chloe", "But WHAT does the nexus do? You have not told me, except that it can help us getting out of here. Also, how do you know about it in the first place?" },//3
        { "ChloeIndifferent", "RayneGemForm", "Azure", "Right! So… The nexus is basically a device to manipulate space. It will send the user towards another location. In this case, this Nexus will send us up closer to the surface. I was trapped inside the Core for a looooonnggg.... time. So I did some scouting~! However..." },//4
        { "ChloeAnnoyedTalking", "RayneGemForm", "Chloe", "I am not going to like the next part, do I...?" },//5
        { "ChloeIndifferent", "RayneGemForm", "Azure", "Nop. This baby here will send us up… Inside the maze made by Labyssal if that note is to be believed. In order to get out completely, we need to find the OTHER Nexus on the next floor and activate it. " },//6
        { "ChloeAnnoyedTalking", "RayneGemForm", "Chloe", "And to do that, we need to purify the Energy Crystals that feed power to the Nexus… Crystals that might be far one from the other…. Wonderful" },//7
        { "ChloeAnnoyed", "RayneGemForm", "Azure", "Don’t be grumpy dear~ I will be there with you all the way!" },//8
        { "ChloeAnnoyedTalking", "RayneGemForm", "Chloe", "You are a floating rock, what can you do?!" },//9
        { "ChloeAnnoyed", "RayneGemForm", "Azure", "You underestimate me hehe…. I was not able to do this due to the lack of magical energy, but now with a nexus to give it to me…. Watch!" },//10
        { "ChloeCurious", "", "Azure", "Transfooooormmmm…… AAH!!! " },//11
        { "ChloeIndifferentTalking", "", "Chloe", "Wh…What the heck?!" },//12
        { "ChloeCurious", "RayneHappyTalking", "Azure", "TADA~!! Back and good as new~!" },//13
        { "ChloeIndifferentTalking", "RayneSmirk", "Chloe", "You…. You were a rock!! How did you get back your body?! Which begs the question of why try to steal mine if you can do this!" },//14
        { "ChloeIndifferent", "RayneHappyTalking", "Azure", "I didn’t get back my body. This is a body made of hard light using mana. I am just a projection so… I am technically still a rock~! But I will more comfortable like this. And just nte that the only way I was able to float and talk with you before is because your body gave me mana, energy, when we touch, but not enough to do this" },//15
        { "ChloeIndifferentTalking", "RayneSmirk", "Chloe", "I see… And your clothes?" },//16
        { "ChloeCurious", "RayneIndifferentTalking", "Azure", "Still working on it. Anyway, that is not important! Are you ready or not?" },//17
        { "ChloeIndifferentTalking", "RayneSmirk", "Chloe", "Yeah yeah… There is not other choice, but sure I am" },//18
        { "ChloeIndifferent", "RayneSmirk", "Tutorial", "(When you are ready, approach the Nexus and interact with it. You will be teleported towards the main area, where you must find and purify the energy crystals in the maze. Once that is done you can exit the maze via the local Nexus. As a secondary objective, you can try to collect all the collectable record notes in the map. They will influence the ending of the game)" }//19
    };

    #endregion

    #region Lore

    private int nbLores = 19;

    string[,] Lore1 = new string[14, 4]
    {
        //1
        {"ChloeCurious", "RayneGemForm", "Note", "<i><b>The note is a torn page of what might have been a diary. It is dirty and degraded due to time and humidity. There are traces of blood and dirt on it, yet the text is legible and organized. Clearly the writer was in no rush nor danger when they wrote this</b></i>"},
        //2
        {"ChloeCurious", "RayneGemForm", " Note ", "<i>To the lucky or unlucky ones that find this, this a record of my final thoughs… since I will die soon. We were five when we entered Labyssal. People say that Labyssal is a maze full of treasures and opportunity. That it is possible to become one of the richest men in the kingdom with a single visit</i>"},
        //3
        {"ChloeCurious", "RayneGemForm", " Note ", "<i>I curse them now. All of those that told us how rich we would become, how adventurous would it be, how heroic we would be</i>"},
        //4
        {"ChloeCurious", "RayneGemForm", " Note ", "<i>The stories about richesses were true, without a doubt. What they did not told us was that once inside, there is no coming back. Things were easy at the beginning, we found some values and got excited, so we continued. It was a trap. </i>"},
        //5
        {"ChloeCurious", "RayneGemForm", " Note", "<i> This place is alive, and it can think. I always felt watch. Some rooms or entrances disappear sometimes or change shape to confuse people. We got lost in no time. Monsters and demons appear at the most unfortunate times, as if they always know where and when to attack</i>" },
        //6
        {"ChloeCurious", "RayneGemForm", " Note", "<i>I lost my friends one by one. All of them from suddenly and shocking deaths to the horrible and slow ones. I should be dead, but something tells me that <s>this place</s> Labyssal enjoys my despair. I am only alive for its entertainment. </i>" },
        //7
        {"ChloeCurious", "RayneGemForm", " Note", "<i>This place is big. VERY big. Which is why most of the people that come here use the ancient teleporters build by the Zenon Empire to move around. It is lost technology, but basically, each teleporter is connected to three crystals that serve as sources of energy. Then you touch it and think about a destination or a general direction where another teleporter might be</i>" },
        //8
        {"ChloeCurious", "RayneGemForm", " Note", "<i>I used the one on the top floor to try to escape to the surface. It teleported me here, a level lower, instead. Probably the maze influenced it with its corruption to change the destination. The one on this floor is not working, due to the corruption, again. Curse this place. Not doubt Labyssal is responsible for it. Do give me hope… and to pull me deeper</i>"},
        //9
        {"ChloeCurious", "RayneGemForm", " Note", "<i>Now I am hunted. Like a beast playing with their prey. I will miss my friends. I mourn for them. I will miss my wife and my-</i>"},
        //10
        {"ChloeSadTalking", "RayneGemForm", " Chloe", "It ends here… I wonder if I was-"},
        //11
        {"ChloeSad", "RayneGemForm", " Azure", "Don’t finish that sentence, Chloe. You were not yourself, and it might not have been you but another of the countless creatures down here"},
        //12
        {"ChloeSadTalking", "RayneGemForm", " Chloe", "Right…"},
        //13
        {"ChloeIndifferent", "RayneGemForm", " Chloe", "Most importantly, we now know that there might be a way out using another crystal Nexus… Those teleporters as this guy calls them, on the top floor. So, we better get going before the hive, Labyssal now that we have a name, gets a grip of the one here!"},
        //14
        {"ChloeAngryTalking", "RayneGemForm", " Chloe", "Got it!"}
    };

    string[,] Lore2 = new string[9, 4]
    {
        //1
        {"ChloeCurious", "RayneIndifferent", "Note", "<i><b> A small diary from one of the skeletons near by has a worn-out cover. The pages inside are damage and unreadable due to what might be dry blood. However, there is a page that is still readable</b></i>"},
        //2
        {"ChloeCurious", "RayneIndifferent", "Note", "<i>Day 4 of the 3rd month 1052 AZE</i>"},
        //3
        {"ChloeCurious", "RayneIndifferent", "Note", "<i>The mining prospecting expedition is not going well. A lost 4 today due to a trap that the fool of Nick activated. The Guild of Adventurers really did a poor job at selecting some people for this mission. However, they at least succeed on keeping most of the prospectors alive. </i>"},
        //4
        {"ChloeCurious", "RayneIndifferent", "Note", "<i>The most recent discoveries indicate that perhaps the stories are true. Labyssal, although mostly known for its deadly maze, has a lot of caves rich on mana crystals. The pureness on mana in the crystals is really promising. Such pureness allows for high circulation of mana and storage, which can be used as superconductors or large batteries of mana</i>"},
        //5
        {"ChloeCurious", "RayneIndifferent", "Note", "<i>Perhaps it is as historians said, and the Zenon Empire was able to reach their strength and superior technology thanks to the access of such crystals. They say that before been a maze, before The Black Wave and The Crusades, it was the largest mine of mana crystals in the continent, and probably the world. </i>" },
        //6
        {"ChloeCurious", "RayneIndifferent", "Note", "<i>Labyssal is a gigantic complex of caves connected to multiple major natural ley lines. This fact might be the reason the environment and air is always supercharged with mana. This might be also why the place might have been able to develop a sense of intelligence or sentient, although there is not enough proof that this is the case</i>" },
        //7
        {"ChloeCurious", "RayneIndifferent", "Note", "<i>Theories and stories aside, the fact remains that this place shows a lot of promise. And the minerals down here might allow civilization to rediscover the ancient technologies of the Zenoans. Now, if there was a way to clean the corrupt mana in the crystals…</i>" },
        //8
        {"ChloeCurious", "RayneHappyTalking", "Azure", "Fascinating, it always surprised me how potent the crystals in the ground were and on the devices here, now I know it is the norm when it comes tot his place. And thinking that people used to mine here… I wonder what happened?"},
        //9
        {"ChloeIndifferentTalking", "RayneThinking", "Choe", "I wonder about some of the thinks he mentioned. Like that Zenon Empire or the other events like that dark wave or crusades. Perhaps we might find more information from other records or diaries"}
    };

    string[,] Lore3 = new string[9, 4]
    {
        //1
        {"ChloeCurious", "RayneIndifferent", "Note", "<i><b>A diary that seems to have been torn apart. However, compared to other objects found, this one seems to have relatively newer. The pages appear to be enchanted to resist erosion and liquids. And while most of the pages were torn out, a few remain</b></i>"},
        //2
        {"ChloeCurious", "RayneIndifferent", "Note", "<i>Day 4 of expedition, 1318 AZE – Entry 17 by Professor Jones</i>"},
        //3
        {"ChloeCurious", "RayneIndifferent", "Note", "<i>Today was a productive day. Thanks to special gems made to absorb mana, I was able to clean up one of the power crystal stations in the maze. This allow me in turn, to see more clearly the connection that the station created with the nearby spatial disrupter and see how the circulation of mana works</i>"},
        //4
        {"ChloeCurious", "RayneIndifferent", "Note", "<i>It is quite fascinating to see and investigate the ancient technology made by the Zenon Empire. Basically, the power stations are large crystals able to collect the natural mana in the air like a sponge. Then, once they are charged, they will create a remote connection with the crystal receptors on the spatial disrupters and send all that mana to them</i>"},
        //5
        {"ChloeCurious", "RayneIndifferent", "Note", "<i>Each spatial disrupter has three receptors, and each of them is not only use receive energy, but also to calculate a coordinate! Then the central crystal creates a spatial void in a set of coordinates X, Y and Z given respectively by each receptor, and fills that void, that vacuum with the user.</i>" },
        //6
        {"ChloeCurious", "RayneIndifferent", "Note", "<i>Which means that the so called teleported does not transport things or people from a place to another, but instead swaps the space between two places, brilliant!</i>" },
        //7
        {"ChloeCurious", "RayneIndifferent", "Note", "<i>But it is a complex and delicate operation. The air is naturally corrupted, and the power stations absorb it. This in turn leads to the power stations been unable to create a connection due to the chaotic nature of corruption. Moreover, it can also lead to bad calculations from the receptors, or worst, having the results been manipulated by the maze…</i>" },
        //8
        {"ChloeCurious", "RayneHappyTalking", "Azure", "You know… You have to love the people that do research. I learned more from this paper that years of scouting gave me~!"},
        //9
        {"ChloeContentTalking", "RayneSmirk", "Chloe", "I agree. This was quite informative. I wonder he made it out…"}
    };

    string[,] Lore4 = new string[8, 4]
    {
        //1
        {"ChloeCurious", "RayneIndifferent", "Note", "<i><b>A torn page intact and of good quality. It sems to be enchanted. A familiar writing and name</b></i>"},
        //2
        {"ChloeHappy", "RayneHappyTalking", "Azure", "Professor Jones~! I see you grace us with your presence again. Tell us your wise words~!"},
        //3
        {"ChloeContent", "RayneHappy", "Note", "<i>Day 5 of expedition, 1318 AZE – Entry 20 by Professor Jones</i>"},
        //4
        {"ChloeContent", "RayneHappy", "Note", "<i>Now, since I am planning to make a book out of this diary, I realized that some history lessons would be useful for future readers. Since the most recent entries cover ancient technology, lets talk about the Zenon Empire</i>"},
        //5
        {"ChloeContent", "RayneHappy", "Note", "<i>Before its formation, it was known as The Kingdom of Zenori. A small below average kingdom during a time where our supercontinent was broken down in hundreds of small and large nations. It is said that The Kingdom of Zenori found the cave complexes of what is known today as Labyssal and mined the rich crystals inside it</i>"},
        //6
        {"ChloeContent", "RayneHappy", "Note", "<i>This in turn allow the kingdom to increase their magical capabilities, develop new and revolutionary technologies, and increase their military might exponentially. When The First Continental War began in the year 21 BZE, the strength of the kingdom was put to the test. At the end, they were victorious at the end of the long 20 year long war. Then the following year, year 0 of our calendar, the Zenon Empire was born </i>" },
        //7
        {"ChloeContent", "RayneHappy", "Note", "<i>Hence AZE stands for After Zenon Empire, and BZE stands for Before Zenon Empire. There is no doubt that their impact has been enormous to this day. After the formation of the empire, The Conquest, the next most important period in history began. For the next century, the empire engaged on a campaign of conquest through the supercontinent, until at last, all the supercontinent was under its banner</i>" },
        //8
        {"ChloeSad", "RayneSad", "Note", "<i>The following centuries were marked by cultural and technological progress, all thanks to the valuable crystals in Labyssal. Then, the Black Wave happened on 601 AZE, the mines and access to the crystals were lost, and the empire collapsed decades later. All the technology and progress were lost in the following wars and crises, and even today we are unable to replicate them. Hence, that is why they are called ancient technologies</i>" }
    };

    string[,] Lore5 = new string[5, 4]
    {
        //1
        {"ChloeCurious", "RayneIndifferent", "Note", "<i><b>A note in a script made of leather. It certainly explains why it is readable and undamaged by humidity. However, it seems to be worn out by time</b></i>"},
        //2
        {"ChloeCurious", "RayneIndifferent", "Note", "<i>I am going to die here, in this cursed place. At the same place some of my ancestors fought and died at during The Fifth Crusade. Yet, the alternative might be worst. Monsters and a maze are terrifying, but humans can be more so</i>"},
        //3
        {"ChloeCurious", "RayneIndifferent", "Note", "<i>I am... was a soldier. Now I am a deserter. But before that, I was an adventurer. I hunted monsters... not humans. When The Second Continental War started 3 years ago, some people said it was going to be quick. A glorious war. And that by the end of it, a new successor to the fallen Zenon Empire would remain standing</i>"},
        //4
        {"ChloeCurious", "RayneIndifferent", "Note", "<i>The fools. Once the corruption was beaten back, onces the demons were driven away, some would think that peace would reign after a century and a half of war. But no! The fools. Now, they decided to start a new one not even 3 generations later. The fools. Monarchs and leaders wanting to inherit the glory and prestige of the Zenon Empire... wanting to unify the continent that facture after it collapsed... Not even trying to talk about it... Thus plunging us into a free for all war in all the continent...</i>"},
        //5
        {"ChloeCurious", "RayneIndifferent", "Note", "<i>The fools!!!</i>" }
    };

    string[,] Lore6 = new string[11, 4]
    {
        //1
        {"ChloeHappy", "RayneHappyTalking", "Azure", "Oh look, the sequel~! Although it is part of the same entry 20 of professor Jones... Guess we found the missing part about the empire"},
        //2
        {"ChloeContent", "RayneHappy", "Note", "<i>... By irony, the place that caused the downfall of the Zenon Empire is the same place that keeps its technology and history from completely disappearing after the chaos that followed. Something tells me that Labyssal likes to collect things as a hobby. Understandable since I am a man of culture and knowledge as well! But anyway, back to the topic</i>"},
        //3
        {"ChloeContent", "RayneHappy", "Note", "<i>The cause for the collapse of the Zenon Empire was The Black Wave. The Black Wave was an explosion of infective corruption that transform victims, that been animals, people or even insects, into monsters and demons. The epicenter was Labyssal, which at the time was the main and largest mine of mana crystals in the empire. Now, corruption by itself was nothing new. Natural mana is a form of energy that can be found in certain concentrations on the air. For a long time, it was consider that corruption was a state where mana is more volatile and harder to control. </i>"},
        //4
        {"ChloeContent", "RayneHappy", "Note", "<i>It is say that negative emotions can affect the mana and increase its chaotic nature. Studies have shown that, although it is true, the effect emotions have on natural mana, even in places with high levels of distress was minimal. But that was for ordinary natural mana in the air. The atmosphere in the mines was far from ordinary. The natural mana in the air has a very large concentration. Mana can act as a conductor at high concentrations, thus the effect of emotions was larger.</i>"},
        //5
        {"ChloeContent", "RayneHappy", "Note", "<i>Centuries before The Black Wave, corruption had been growing slowly but continuously in the mines. This of course lead to multiple sudden accidents in the mine that killed many. Yet, it was never a cause for alarm since it never got worst than that. Today, there is solid evidence that Labyssal is fully sentient, an intelligent hivemind. But there are theories that is was not always the case. That it was sentient, and chaotic, but never actively hostile. We don't know what happen that day, but at the 16th day of the 6th month, 605 AZE, a day that will live in infamy, an explosion of corruption surged from the Labyssal</i>" },
        //6
        {"ChloeIndifferent", "RayneIndifferent", "Note", "<i>This corruption was different. It was very visible and black. Black clouds expanded in the sky and expanded for kilometers. Black rain fell from them, and all things that touch it changed. Entire villages and cities were wipe out. Its habitants transformed into hostile lifeforms. Same for the animals and all other lifeforms. At the heart of the empire, a wave of corruption that contaminates and transforms its receivers appeared and started expanding. Soon, the empire was at war. Within the next decades, a war for the survival of civilization develop. The empire lost its capital and multiple major cities. Then, they succeed on containing the threat</i>" },
        //7
        {"ChloeIndifferent", "RayneIndifferent", "Note", "<i>Militarily, the empire was doing good. It succeed on mounting a defense and started counter attacking to recapture territory or defend itself from enemy armies. Those counter attacks are known today as the First and Second Crusade. However, politically, the empire was doom. They lost their center of power, and were forced to decentralize to continue the fight. This trend continue until the central government had little real political power. When some territories began to FORMALLY declare their independence, others followed. Then, in 642 AZE, the empire was declared as officially dead</i>" },
        //8
        {"ChloeIndifferent", "RayneIndifferent", "Note", "<i>The territories that declared independence did not do it for a desire of freedom, but simply by practical reasons. The central government had little to no power. The empire died long before that, and everyone knew it. In the end, it is the nations that came out of the fractured empire that drove the corruption and hostile lifeforms away to Labyssal during the next century. The Fifth Crusade, also called the Last Crusade, was the last campaign in 748 AZE to purify the corruption from the continent. They reached Labyssal and even entered</i>"},
        //9
        {"ChloeIndifferent", "RayneIndifferent", "Note", "<i>You see, Labyssal was not always a maze. Not even after the beginning of The Black Wave nor the crusades. It is only until the crusaders got at its door step that it started to transform itself into a mighty maze to defend itself. The Fifth Crusade failed to destroy Labyssal, yet Labyssal never tried to exert its influence outside of its cave ever again. Which is why some call it a success</i>"},
        //10
        {"ChloeIndifferent", "RayneIndifferent", "Note", "<i>What was NOT a success, was the shit show that followed. Now that the threat towards civilization was gone, the remaining nations looked at the political body of the empire not with sadness nor happiness, but with nostalgia and desire. What followed was a cold war between all nations, who all consider themselves the rightful successors of the empire, to claim hegemony in the supercontinent. Then in 856 that cold war got really hot, and the Second Continental War, lasting almost half a century... Without a winner</i>"},
        //11
        {"ChloeIndifferentTalking", "RayneIndifferent", "Chloe", "Yeah... A shit show indeed"}
    };

    string[,] Lore7 = new string[8, 4]
    {
        //1
        {"ChloeCurious", "RayneIndifferent", "Note", "<i><b>An old diary covered in dried blood and dirt. Almost nothing survived except for a page that is barely readable</b></i>"},
        //2
        {"ChloeCurious", "RayneIndifferent", "Note", "<i>3rd Day of the 2nd Month, 751 AZE - Year 3 of the Fifth Crusade</i>"},
        //3
        {"ChloeContent", "RayneIndifferent", "Note", "<i>We were winning. As in past sentence. Because we are now losing... Badly</i>"},
        //4
        {"ChloeIndifferent", "RayneIndifferent", "Note", "<i>We reached the enemy stronghold. The epicenter of what is now officially called The Black Wave. There used to be a mine here and large caves. And I don't mean that as if everyone that used to work here is dead... But because there is not a mine nor caves anymore!</i>"},
        //5
        {"ChloeIndifferent", "RayneIndifferent", "Note", "<i>Now there is a maze. With corridors only wide enough for 3 men to go shoulder to shoulder. Large rooms that might as well be dead traps, since the entrance seals itself when enough of us pass through. Monsters that come from the walls and attack from blind spots...</i>" },
        //6
        {"ChloeIndifferent", "RayneIndifferent", "Note", "<i>We will not win this. No way. A siege will be useless since our enemy will not start, since it has infinite mana to feed itself. So it will be containment then.</i>" },
        //7
        {"ChloeIndifferent", "RayneIndifferent", "Note", "<i>I am already pulling my men back. No one wants to be in a place where darkness itself swallows people. A large labyrinth that in a place famous for its pure and shinning crystal, now representing abyss itself! Some men are giving it the name Labyssal, which.... well.... to be honest, it does not sound too bad</i>" },
        //8
        {"ChloeIndifferentTalking", "RayneIndifferent", "Chloe", "So that is how it got its name.... interesting"}

    };

    string[,] Lore8 = new string[6, 4]
    {
        //1
        {"ChloeCurious", "RayneIndifferent", "Note", "<i><b>An old book. It has degraded due to time, and it has been mostly destroy except for the last entry. However, the letters on the last entry are pretty messy, which means the writer was under a lot of stress, in a rush, or both. There is dried blood covering the page</b></i>"},
        //2
        {"ChloeSad", "RayneSad", "Note", "<i>To anyone reading this. I am a guard of the mines. It is the 16th of the 6th month.</i>"},
        //3
        {"ChloeSad", "RayneSad", "Note", "<i>It happen suddenly. Shadows grew darker and began shallowing people alive. Walls and tops crumble as entire sections of the mine collapsed</i>"},
        //4
        {"ChloeSad", "RayneSad", "Note", "<i>The corruption began corrupting everyone entirely to the point of transforming them into monsters!</i>"},
        //5
        {"ChloeSad", "RayneSad", "Note", "<i>The door will not hold. I will die soon. If someone finds this, tell my wife Marta I lov...</i>" },
        //6
        {"ChloeSadTalking", "RayneSad", "Chloe", "He must have died then. This is the day where it began... But there is not explanation. What could have happened?" }
    };

    string[,] Lore9 = new string[5, 4]
    {
        //1
        {"ChloeCurious", "RayneIndifferent", "Note", "<i><b>Another old book. But this one appears to be in better condition and organized. A record</b></i>"},
        //2
        {"ChloeIndifferent", "RayneIndifferent", "Note", "<i>Summary of minning operations for year 549 AZE</i>"},
        //3
        {"ChloeIndifferent", "RayneIndifferent", "Note", "<i>Like the previous years, the corruption in the air causes problems here and there. We receive some equipment this year to improve purification of mined crystals before sending them to to be further processed. We also receive improved energy collectors that are more efficient on collecting mana from the air and filtering the chaotic aspects of it. Of course, that will not help much with the accidents that might happen and the workers that will be inevitably lost during operations.</i>"},
        //4
        {"ChloeAngry", "RayneAngry", "Note", "<i>Luckly, the slaves workers come cheap. Although avoiding looses is preferable to avoid the lost of productivity during the time it takes to replace them. There is currently a plan to put the less productive ones into the more dangerous postions to maximize efficiency</i>"},
        //5
        {"ChloeAngryTalking", "RayneAngry", "Chloe", "No wonder the place got corrupted... With cynicals like him running it" }
    };

    string[,] Lore10 = new string[4, 4]
    {
        //1
        {"ChloeCurious", "RayneIndifferent", "Note", "<i><b>Very old diary. It was hidden in a small hole close to the wall. Very few full and readable sentences remain</b></i>"},
        //2
        {"ChloeIndifferent", "RayneIndifferent", "Note", "<i>It has been 3 years since the new town was founded, and 8 years since the caves were discover. My family and me moved recently here when we heard about the mine.</i>"},
        //3
        {"ChloeIndifferent", "RayneIndifferent", "Note", "<i>It is an oportunity like unlike any other. Not only it is a better job, but it is also an opportunity to improve the country!</i>"},
        //4
        {"ChloeHappy", "RayneIndifferentTalking", "Azure", "Damn... This diary most be really old. It is from an era long forgotten!" }
    };

    string[,] Lore11 = new string[6, 4]
    {
        //1
        {"ChloeHappy", "RayneHappyTalking", "Azure", "Yay~! Our favorite professor is back! At this point I am a fan...~"},
        //2
        {"ChloeContent", "RayneHappy", "Note", "<i>Day 6 of expedition, 1318 AZE – Entry 23 by Professor Jones</i>"},
        //3
        {"ChloeContent", "RayneHappy", "Note", "<i>It has recently come to my attention that while I talked a lot about the Zenon Empire, I have yet to go much into detail about the Kingdom of Zenori. This shall be rectified immediately!</i>"},
        //4
        {"ChloeContent", "RayneHappy", "Note", "<i>We already know that the kingdom got its power from the mines, but what about the state of the mines themselves. Well, following surviving historical records, it can be said that no corruption existed at the beginning of mining. People saw the opportunity that this mine could provide, and soon, they began to go there in search of opportunity. Soon, not even 5 years after its discovery in 131 BZE, there was already a town on there! After some paper work, the town was officially recognized as the mining town of Prisma. It is from that name that Labyssal got its first name, The Caves of Prisma, and later The Mine of Prisma.</i>"},
        //5
        {"ChloeContent", "RayneHappy", "Note", "<i>Like negative emotions, positive emotions do affect natural mana in the air. They make it more stable and tamable. Records say that the mine itself had a higher output of crystal grown during some of the early years.</i>" },
        //6
        {"ChloeSad", "RayneSad", "Note", "<i>Those were prosperous times... Then the First Continental War happened in 21 BZE, and a few years later during the war, Prisma burnt</i>" }
    };

    string[,] Lore12 = new string[6, 4]
    {
        //1
        {"ChloeIndifferent", "RayneIndifferent", "Note", "<i><b>An old diary abandoned on a corner, hidden from view. It is very old and worn out, but most notably, it has what looks like burn marks</b></i>"},
        //2
        {"ChloeSad", "RayneSad", "Note", "<i>I got my babies out of the town... but my husband did not make it</i>"},
        //3
        {"ChloeSad", "RayneSad", "Note", "<i>It has been 12 years since this war started. From what new we got it seems we are winning at this point. But our ennemies will to concede defeat. They know how important is this place to the kingdom... So they send a small army to raid us</i>"},
        //4
        {"ChloeSad", "RayneSad", "Note", "<i>So now my husband is death, buying us time to escape and take refuge in the mines</i>" },
        //5
        {"ChloeSad", "RayneSad", "Note", "<i>I am mourning... The families around me are mourning... I can see dark shades in the crystals in the walls, which probably means the earth itself is mourning</i>" },
        //6
        {"ChloeSad", "RayneSad", "Note", "<i>Prisma is mourning</i>" }
    };

    string[,] Lore13 = new string[6, 4]
    {
        //1
        {"ChloeIndifferent", "RayneIndifferent", "Note", "<i><b>An old book is found... One about history. But it seems very few pages remain intact</b></i>"},
        //2
        {"ChloeIndifferent", "RayneIndifferent", "Note", "<i>This book not only informs about history, but also is a way to celebrate the soon 100th year anniversary since the proclamation of the Zenon Empire</i>"},
        //3
        {"ChloeIndifferent", "RayneIndifferent", "Note", "<i>It might have been only a few generations, but even today, there are physical scars in our empire from the Continental War. It was a long and brutal war, which was caused primarily by the jealousy of the other bigger kingdoms at the time. This jealousy comes evidently from out rapid advances on technology and magic.</i>"},
        //4
        {"ChloeSad", "RayneSad", "Note", "<i>The burning of Prisma is proof enough that said jealousy is real. Today, Prisma is a ghost of its formed glory. At the end of the war, when out enemies asked for mercy, for a conditional surrender, we gave none. They got the Prisma treatment as some call it. By the end of the 18 years war, our territory was 5 times larger, and our enemies were weak or completely gone. But with new territory, come new subjects. We now have to integrate them in our society, but our bureaucracy is quite and our society quite meritocratic... Unless you were a mage</i>" },
        //5
        {"ChloeSad", "RayneSad", "Note", "<i>During the was, natural born mages were the strongest opposition against the Zenori Kingdom. Because of that, a certain hatred and fear of mage was born. During the war, all mages were to be forcebly enlisted on the empire as assets or sent to reserves to keep normal people and mages separated. Those laws are still on effect tot his day.</i>" },
        //6
        {"ChloeAngry", "RayneIndifferentTalking", "Azure", "Well... Now that is depressing. Since our type was not very well liked back in the day..."}
    };

    string[,] Lore14 = new string[13, 4]
    {
        //1
        {"ChloeIndifferent", "RayneIndifferent", "Note", "<i><b>A damaged diary. It is old, but in good condition. It seems an enchantment has help to its preservation....Some of its pages were torn apart however</b></i>"},
        //2
        {"ChloeCurious", "RayneIndifferentTalking", "Azure", "Oh look! It is profess-... Or not. Nop, it is not the same material nor the same template as professor Jones, entries. Yet... It has a similar enchantment..."},
        //3
        {"ChloeCurious", "RayneIndifferent", "Note", "<i>Day 9 of 6th Month, 603 AZE</i>"},
        //4
        {"ChloeSad", "RayneSad", "Note", "<i>Another day, another shit show... It has been two years since they brought me to this mine. It has been 2 years since my days as a slave in hell started...</i>" },
        //5
        {"ChloeSad", "RayneSad", "Note", "<i>Not only the days are crap because of the large amount of work... Like always, everyone is after me to make me suffer for the unforgivable crime of been a mage. The bastards... Never mind that it is thanks to me that the warden or this bloody mine does not have to send their damaged equipment to the capital and wait for repairs! Because I am able to repair them, that is how bloody good of a mage I am!</i>" },
        //6
        {"ChloeSad", "RayneSad", "Note", "<i>Never mind that I am good at purifying stuff and keeping the corruption away! Not only I don't a thanks or any appreciation... The bullying only gets worst!! I guess the only reason they never go into the extreme is because they know how valuable I am. But like hell I will feel grateful!!</i>" },
        //7
        {"ChloeSad", "RayneSad", "Note", "<i>I am... a daughter... a proud Azurheard mage. My family might be death because of the empire, but as the sole survivor, I am determined to carry their legacy. </i>" },
        //8
        {"ChloeSad", "RayneSad", "Note", "<i><b>Signed, Azulheard</b></i>" },
        //9
        {"ChloeSadTalking", "RayneSadTalking", "Azure", "!!!!... Azur.... heard..." },
        //10
        {"ChloeCurious", "RayneThinking", "Chloe", "Azure? Are you alright?" },
        //11
        {"ChloeCurious", "RayneSadTalking", "Azure", "Azul... Azulheard.... I.... My name is Rayne Azulheard" },
        //12
        {"ChloeSadTalking", "RayneSad", "Chloe", "Soo... That girl..." },
        //13
        {"ChloeSad", "RayneSadTalking", "Rayne", "Yeah... I think it is me. I don't really remember it, but I feel emotional when thinking about those scenes of her alone. Maybe... Maybe the memories will come. To know who Rayne Azurheard was..." }
    };

    string[,] Lore15 = new string[5, 4]
    {
        //1
        {"ChloeContent", "RayneHappy", "Note", "<i><b>A paper a bit dirty, but in good condition </b></i>"},
        //2
        {"ChloeContent", "RayneHappy", "Note", "<i>Day 7 of expedition, 1318 AZE – Entry 28 by Professor Jones</i>"},
        //3
        {"ChloeContent", "RayneHappy", "Note", "<i>There is a rumor that the burning of Prisma was the cause to the beginning in the grown of corruption. I disagree with the notion. However, I do agree that the burning of Prisma had indirect consequences that let to a grown of corruption. Basically, the burning of Prisma let to the dead of many of the miners... Which in turn let the door open to the use of slaves as miners.</i>"},
        //4
        {"ChloeContent", "RayneHappy", "Note", "<i>Slavery was not new in the Kingdom of Zenori, but it was not common neither. After the First Continental War, the kingdom needed labor on the mines to keep them functioning. Initial recruitment campaigns worked for a while, until they did not. You see, the job of a miner was something people of the empire were ok during the time where the country was poor and was developing. Now... People are not interested anymore. So the government turns to slave labor to fill its needs. </i>" },
        //5
        {"ChloeSad", "RayneSad", "Note", "<i>Then, with time, more and more slaves made a larger portion of the work force, creating a more negatively charged environment, which in turn let to the rise of corruption levels in the mine</i>" }
    };

    string[,] Lore16 = new string[17, 4]
    {
        //1
        {"ChloeSad", "RayneSad", "Note", "<i><b>A torn page from a diary. It is old, but in good condition. At the button, the name Azulheard is signed</b></i>"},
        //2
        {"ChloeSadTalking", "RayneSad", "Chloe", "Azu-.... I mean Rayne. You don't have to read it if you want to. I can see the tension on your body... projection"},
        //3
        {"ChloeSad", "RayneAngryTalking", "Rayne", "Sorry, no can do Chloe. If this is the past me, and the date is of any indication, this might be the clue telling us what happen THAT day"},
        //4
        {"ChloeAngry", "RayneAngry", "Note", "<i>Day 19 of 7th Month, 604 AZE</i>"},
        //5
        {"ChloeAngry", "RayneAngry", "Note", "<i>Today... Was a bad day. A shitty day. They insulted me, which is something I can take. But then... they insulted my family, and most importantly, mom. </i>" },
        //6
        {"ChloeAngry", "RayneAngry", "Note", "<i>So I fought back, and gave the bully a bloody nose for its audacity! I loved my mom. She was always good and kind to me, yet always with a flashy and funny personality. Some people in the village said I am like my mom when she was a girl, a proud and egocentric person hehe...</i>" },
        //7
        {"ChloeSad", "RayneAngry", "Note", "<i>I miss her so much. I sometimes have nightmares when she died in my arms while giving me a Azur Core, our family's heirloom. She told me to always keep it close when feeling the wait of despair and hopelessness on me. That she will always be with me at anytime. </i>" },
        //8
        {"ChloeCurious", "RayneSad", "Note", "<i>I don't fear my heirloom been stolen. It has powerful enchantments that make anything, and I mean ANYTHING including this creepy place that is clearly alive from seeing it and taking it! Only people of the Azulheard blood will be able to see pass them. Which is why I always keep it with me even at work, since nobody is capable of seen it.</i>" },
        //9
        {"ChloeSad", "RayneSad", "Note", "<i>One day... When the opportunity comes, I will get out of here and remake my clan. I have grown stronger since I got here and I am better at controlling magic. The other reason is because... I don't like it here. Nobody but me has noticed... but this place is alive. It is sentience, all in one. I am quite sure it is responsable for a lot of the accidents, yet it does not have hostile intent nor killing intent. It is... passive. I have yet to be able to communicate with it, yet I really don't want to.</i>" },
        //10
        {"ChloeCurious", "RayneThinking", "Note", "<i>If we enter communication... I fear its sheer power will overwhelm me. In the worst case scenario, if I die, I might have to use the forbidden spell stored inside the Azure Core. A spell that trumps death. A soul linking. Basically, before death, I will link my soul to the Core, and after dying, my soul will be stored inside the Core. And if I am lucky... I mind find a body to act as vessel. But knowing my luck, it might has well be decades before someone find me...</i>" },
        //11
        {"ChloeSad", "RayneThinking", "Note", "<i><b>Signed, Azulheard</b></i>" },
        //12
        {"ChloeFocus", "RayneSadTalking", "Rayne", "I guess... now it all makes sense. I probably died here and in a last act of rebellion, I sealed myself away and... Chloe? Are you alright?" },
        //13
        {"ChloeIndifferentTalking", "RayneIndifferent", "Chloe", "Uh? Oh yeah yeah... I was simply thinking about something you wrote in your diary, about only people of Azulheard blood been able to see the Azure Core"},
        //14
        {"ChloeIndifferent", "RayneIndifferentTalking", "Rayne", "Yeah... You heard my past self, my direct family died. Yet, nothing is mention about indirect family. You know, cousins, uncles or aunts... " },
        //15
        {"ChloeCurious", "RayneSmirkTalking", "Rayne", "....bastards" },
        //16
        {"ChloeAnnoyedTalking", "RayneSmirk", "Chloe", "... You are funny one, aren't you?" },
        //17
        {"ChloeSmug", "RayneHappyTalking", "Rayne", "You heard my past self. I am quite egocentric~" }
    };

    string[,] Lore17 = new string[8, 4]
    {
        //1
        {"ChloeIndifferent", "RayneIndifferent", "Note", "<i><b>A book full of entries. It is old and worn out. There are some pages missing, but the last entry in this book stands out</b></i>"},
        //2
        {"ChloeIndifferent", "RayneIndifferent", "Note", "<i>15th Day of the 5th Month, 605 AZE - Warden Boris</i>"},
        //3
        {"ChloeAngry", "RayneAngryTalking", "Rayne", "This date... It is only a day before The Black Wave.... This might give us clues of what happen"},
        //4
        {"ChloeAngry", "RayneAngry", "Note", "<i>The girl is gone</i>"},
        //5
        {"ChloeAngry", "RayneAngry", "Note", "<i>I already questioned the guards outside the mine, and nobody has been her. The crystal censors outside have not detected her neither. However, there is a slave that said he witness the girl go into the forbiden sections of the mine. The sections that are currently seal off after the colossal disaster of yesterday. Hundreds die in that cave collapse. A lot of valuable equipment was lost. I am unsure if trust the word of a slave since they can be unreliable those days...</i>" },
        //6
        {"ChloeAngry", "RayneAngry", "Note", "<i>Anyway, if it was any other girl, this would not be an issue. But she is our only mage, and a good one, in the mine. Without her, we will have to go back at sending repair and maintenance requests. Worst, the process to purify crystals will slow dramatically... I will have to contact the capital with the news and request a new mage. I might also get the girl's personal papers while doing so, since we don't have her personal history papers, nor her first name.</i>" },
        //7
        {"ChloeFocus", "RayneSadTalking", "Rayne", "I guess... I might have escaped and done something nasty before going away...?" },
        //8
        {"ChloeIndifferentTalking", "RayneIndifferent", "Chloe", "Maybe..? But you are here so... Perhaps you did go deeper in Labyssal. We need to find more clues..."}
    };

    string[,] Lore18 = new string[8, 4]
    {
        //1
        {"ChloeIndifferent", "RayneIndifferent", "Note", "<i><b>A torn page from a diary. It is old, but in good condition. A now familar signature, yet... the letters are chaotic, as if the person is distressed</b></i>"},
        //2
        {"ChloeAngry", "RayneIndifferent", "Note", "<i>Day 14 of 5th Month, 605 AZE</i>"},
        //3
        {"ChloeCurious", "RayneSad", "Note", "<i>I lost it... I survived that cave collapse that killed many, including a lot of people that I have a grudge with. I should be happy, but... but I am not! I lost the Azure Core!! I fell from me into the void of a pitfall. My family's legacy... my heirloom... mom's last gift... I will not be able to continue through this without it!</i>" },
        //4
        {"ChloeAngry", "RayneAngry", "Note", "<i>I already made preparations. I am going to search for it, consequences be damn!!!</i>" },
        //5
        {"ChloeSad", "RayneThinking", "Note", "<i><b>Signed, Azulheard</b></i>" },
        //6
        {"ChloeFocus", "RayneSadTalking", "Rayne", "I... lost it? Then how...?" },
        //7
        {"ChloeSadTalking", "RayneSad", "Chloe", "But you when to search for it... so you must have found it and... something happened"},
        //8
        {"ChloeIndifferent", "RayneIndifferentTalking", "Rayne", "Yeah... Perhaps... But... Something does not feel right"}
    };

    string[,] Lore19 = new string[51, 4]
    {
        //1
        {"ChloeIndifferent", "RayneIndifferent", "Note", "<i><b>A torn page. It is old, but still readable. It is a single entry... the last one make in the warden's book</b></i>"},
        //2
        {"ChloeIndifferent", "RayneIndifferent", "Note", "<i>16th Day of the 5th Month, 605 AZE - Warden Boris</i>"},
        //3
        {"ChloeAngry", "RayneAngryTalking", "Rayne", "This is it... At last, there might be some answers to what happen"},
        //4
        {"ChloeAngry", "RayneAngry", "Note", "<i>The girl has been gone for a day... I am already missing her. If only I knew how VALUABLE she was...</i>"},
        //5
        {"ChloeAngry", "RayneAngry", "Note", "<i>I got news from the capital. There will be replacements, but all of them FAR behind in terms of talent and value that the girl had. Azulheard is her family name. I already know that, what is how I and others call her when addressing her. What I did NOT know, was that Azulhard is the family name of a legendary line of mages. With a lot of talent. They fell out of grace due to the anti-mage policies in the last centuries, but they remained talented nonetheless. Worst...</i>" },
        //6
        {"ChloeSadTalking", "RayneSadTalking", "Note", "<i>There are no more Azulheards. The girl was the last of her line. Imperial intelligence confirms it. And if imperial intelligence says so, it is a fact.</i>" },
        //7
        {"ChloeSadTalking", "RayneAngry", "Chloe", "But then how...?"},
        //8
        {"ChloeSad", "RayneSad", "Note", "<i>The last Azulheard lived in a village in the east of the empire. The girl and her parents. There was an insurrection in the area, and the family was suspected to be collabators. But I believe they were simply collateral damage due to their status as mages</i>" },
        //9
        {"ChloeCurious", "RayneSadTalking", "Note", "<i>The girl's parents, Renard Azulheard and Rayne Azulheard died during the skirmish that happened in the village</i>" },
        //10
        {"ChloeSad", "RayneSadTalking", "Rayne", "No... but that would mean...!" },
        //11
        {"ChloeCry", "RayneSadTalking", "Note", "<i>The girl was taken prisoner, marked as a slave and transfered multiple times until she ended here due to her talents</i>" },
        //12
        {"ChloeTears", "RayneSadTalking", "Rayne", "No...No...NO!! This is all wrong... I am... you are...but..." },
        //13
        {"ChloeTears", "RayneSad", "Note", "<i>Chloe Azulheard is still missing today, but I already told the guards in the mine to open their eyes. Surely... a talented girl like her could not perish so easily?</i>" },
        //14
        {"ChloeTearsTalking", "RayneSad", "Chloe", "<i>Sniff...</i> But I did... I died. It is... all coming together now. The memories. A-And... <i>Sniff</i>... From the look of realization in your face... you to right...mom? Your... last moments?" },
        //15
        {"ChloeTears", "RayneSadTalking", "Rayne", ".....Yes. I DID cast the forbiden sleep, soul linker before I died... But..." },
        //16
        {"ChloeTearsTalking", "RayneSad", "Chloe", "...mom?" },
        //17
        {"ChloeTears", "RayneSadTalking", "Rayne", "But I failed... I wanted to... be there with you. I knew you would be alone, so I damn myself to be with you... But once inside, I could not do anything but watch!!! It is only decades later via trial and error that I found a way to project my voice and interact with the world. But I was too late..." },
        //18
        {"ChloeTearsTalking", "RayneSad", "Chloe", "Mom..." },
        //19
        {"ChloeTears", "RayneSadTalking", "Rayne", "I am a failure as a mother... Unable to do anything but watch as my little girl is throw at the wolves. I am starting to remember... how you cried in the nights for salvation... for hope... yet none came. Then you were taken away from me, and I my despair... I forgot about everything. About you.... About... My little girl...." },
        //20
        {"ChloeTearsTalking", "RayneSad", "Chloe", "No... Y-You did your best. It is I who... should apologize- No, not even apologies will do for my unforgivable crimes" },
        //21
        {"ChloeTears", "RayneIndifferentTalking", "Rayne", "...Chloe? What...do you mean" },
        //22
        {"ChloeTearsTalking", "RayneSad", "Chloe", "Because I won mom" },
        //23
        {"ChloeTears", "RayneSadTalking", "Rayne", "...Chloe?..." },
        //24
        {"ChloeCryTalking", "RayneIndifferentTalking", "Chloe", "Because, one day, the 16th of the 6th Month 605 AZE, I fell into a pitfall while searching for the Azur Core. That day, I was dying... I was dying, desperate, and more importantly, furious! " },
        //25
        {"ChloeCry", "RayneIndifferentTalking", "Rayne", "....." },
        //26
        {"ChloeAngryTalking", "RayneIndifferentTalking", "Chloe", "My rage, my determination, my angony.... All of that so that I get to die alone forgotten?!!" },
        //27
        {"ChloeAngryTalking", "RayneIndifferentTalking", "Chloe", "No... That would not do. I refused to... which is why, in my despair, I unintentionally stablish a connection with the hivemind of the caves..." },
        //28
        {"ChloeAngry", "RayneSadTalking", "Rayne", ".....And then you got absorb into-" },
        //29
        {"ChloeAngryTalking", "RayneSadTalking", "Chloe", "... And then I came in top" },
        //30
        {"ChloeSmug", "RayneIndifferentTalking", "Rayne", "What?! But that is not-" },
        //31
        {"ChloeAngryTalking", "RayneIndifferentTalking", "Chloe", "Possible? Perhaps. But I was not lying when I said that I was getting stronger and stronger then. But the most important factor was the state of the hivemind back then. If the hivemind in the caves had a sense of self, I would have lost, overwealm by pure spiritual power. But that was not the case. It was sentience, but lacked coordination, self-preservation and... desire. So I tore it down. I won and came on top" },
        //32
        {"ChloeAngry", "RayneAngryTalking", "Rayne", "But.... It does not make any sense!!! The same day, Labyssal, the hivemind, unless death on all the mine, then caused The Black Wave! If you came on top then why-" },
        //33
        {"ChloeAnnoyedTalking", "RayneSadTalking", "Chloe", "What.... does not make sense mom? I already told you, I came on top. After that, well... I merged with the hivemind. I created coordination, order, self-preservation and.... Imposed my desires"},
        //34
        {"ChloeAnnoyed", "RayneIndifferentTalking", "Rayne", "...then..." },
        //35
        {"ChloeIndifferentTalking", "RayneSad", "Chloe", "In order to create and impose my will while remaining connected to the hivemind, I became administrator... The Queen Administrator. The ruler that is connected to all of her subjects... Yeah, that is right..."},
        //36
        {"ChloeSmug", "RayneSad", "Chloe", "... I am Labyssal"},
        //37
        {"ChloeAnnoyed", "RayneAngryTalking", "Rayne", "... The Black Wave..." },
        //38
        {"ChloeAnnoyedTalking", "RayneAngry", "Chloe", "Oh that... I will admit that it got out of hand. I was quite angry you know? And all my minions agreeing and encouraging me did not help. Still, only a part of me regrets it, while the other part is quite satisfied that we got to destroy the Zenon Empire. Then I got terrified when the Fifth Crusade happen... Hence why I made a maze, to protect myself"},
        //39
        {"ChloeAnnoyed", "RayneAngryTalking", "Rayne", "After that?" },
        //40
        {"ChloeAnnoyedTalking", "RayneIndifferent", "Chloe", "I cool off. With the empire gone, my anger was now unjustified so... I stay here."},
        //41
        {"ChloeContentTalking", "RayneAngry", "Chloe", "But now I found you again mom~! You have returned to me again! Now that I have all my memories, I can get us out of here easily. I will first synchronize back with the Hiv-"},
        //42
        {"ChloeIndifferentTalking", "RayneAngry", "Rayne", "<i> SLAP </i> <i><b>(Rayne slaps Chloe in the face)</b></i>" },
        //43
        {"ChloeIndifferentTalking", "RayneAngry", "Chloe", "... mom...?"},
        //44
        {"ChloeIndifferentTalking", "RayneAngryTalking", "Rayne", "I already failed my daughter. I rather die again than failed her again by allowing herself to be consumed by the abyss once again"},
        //45
        {"ChloeIndifferent", "RayneAngry", "Chloe", "...."},
        //46
        {"ChloeCry", "RayneIndifferent", "Chloe", "....." },
        //47
        {"ChloeTears", "RayneSad", "Chloe", "...<i> Sniff </i>"},
        //48
        {"ChloeTearsTalking", "RayneHappy", "Chloe", "...I am sorry... mommy... I... I was..." },
        //49
        {"ChloeTears", "RayneHappyTalking", "Rayne", "I know Chloe. And... I might be a bad person for forgiving you after hearing what you did, but... I am your mother first and foremost. So I will forgive you and be always there for you... so you are not alone and do something like this again"},
        //50
        {"ChloeTearsHappy", "RayneHappy", "Chloe", "....Yes... I would certainly like that" },
        //51
        {"ChloeTearsHappy", "RayneHappyTalking", "Rayne", "Then... Lets get out of here. Far from the abyss, into this new world"}
    };

    #endregion
}

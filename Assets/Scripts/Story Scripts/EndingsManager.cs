using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EndingsManager : MonoBehaviour
{
    DialogueSystem ds;
    TextArchitect architect1;
    TextArchitect architect2;
    TextArchitect architect3;

    public GameObject CinematicImage;

    public Texture EDImage1;
    public Texture EDImage2;
    public Texture EDImage3;
    public Texture EDImage4;

    public AudioSource EDSound;
    public AudioSource EDSoundtrack;
    public AudioSource EDVoiceline;

    public bool isEndingA;
    public bool isEndingB;
    public bool isEndingC;

    public AudioClip[] EDSoundList;
    public AudioClip[] EDSoundtrackList;
    public AudioClip[] EDVoicelineList;

    public TextArchitect.BuildMethod bm = TextArchitect.BuildMethod.instant;

    private bool startEnding;
    private bool endSoundtrack;
    private int lineNB;

    private string CharacterObjActive;

    void Start()
    {
        startEnding = true;
        endSoundtrack = false;
        CharacterObjActive = "";
        lineNB = -1;

        //Set Architect
        ds = DialogueSystem.Instance;
        architect1 = new TextArchitect(ds._dialogueContainer.dialogueText);
        architect1.buildMethod = TextArchitect.BuildMethod.typewriter;
        architect1.speed = 0.5f;

        architect2 = new TextArchitect(ds._dialogueContainer.dialogueText2);
        architect2.buildMethod = TextArchitect.BuildMethod.typewriter;
        architect2.speed = 0.5f;

        architect3 = new TextArchitect(ds._dialogueContainer.dialogueText3);
        architect3.buildMethod = TextArchitect.BuildMethod.typewriter;
        architect3.speed = 0.5f;

        //Make Cinematic Image Visible
        CinematicImage.SetActive(true);




    }

    // Update is called once per frame
    void Update()
    {
        if (isEndingA)
        {
            if (startEnding)
            {
                startEnding = false;
                CheckAndSwapCinematic(lineNB);
                CheckAndPlaySound(lineNB);
                CheckAndPlaySoundtrack(lineNB);
                lineNB++;
            }
            if (Input.GetKeyDown(KeyCode.Space) && lineNB < EndingA.Length / 4)
            {
                //This statements blocks sounds, soundtracks or Cinematics from swapping until all text is build
                if (!architect1.isBuilding && !architect2.isBuilding && !architect3.isBuilding)
                {
                    CheckAndSwapCinematic(lineNB);
                    CheckAndPlaySound(lineNB);
                    CheckAndPlaySoundtrack(lineNB);
                }


                if (architect1.isBuilding || architect2.isBuilding || architect3.isBuilding)
                {
                    //Allows us to accelerate text typewriting or force it to complete depending on current dialogue box been used
                    if (architect1.isBuilding)
                    {
                        if (!architect1.fasterText)
                            architect1.fasterText = true;
                        else
                            architect1.ForceComplete();
                    }
                    else if (architect2.isBuilding)
                    {
                        if (!architect2.fasterText)
                            architect2.fasterText = true;
                        else
                            architect2.ForceComplete();
                    }
                    else if (architect3.isBuilding)
                    {
                        if (!architect3.fasterText)
                            architect3.fasterText = true;
                        else
                            architect3.ForceComplete();
                    }
                }
                else if (EndingA[lineNB, 1] == "1")
                {
                    architect1.Build(EndingA[lineNB, 2]);
                    CheckAndPlayVoiceline(lineNB, EndingA);
                    lineNB++;
                }
                else if (EndingA[lineNB, 1] == "2")
                {
                    architect2.Build(EndingA[lineNB, 2]);
                    CheckAndPlayVoiceline(lineNB, EndingA);
                    lineNB++;
                }
                else if (EndingA[lineNB, 1] == "3")
                {
                    architect3.Build(EndingA[lineNB, 2]);
                    CheckAndPlayVoiceline(lineNB, EndingA);
                    lineNB++;
                }
                else if (EndingA[lineNB, 1] == "0")
                {
                    architect1.Build(EndingA[lineNB, 2]);
                    architect2.Build(EndingA[lineNB, 2]);
                    architect3.Build(EndingA[lineNB, 2]);
                    CheckAndPlayVoiceline(lineNB, EndingA);
                    lineNB++;
                }
            }
            else if (Input.GetKeyDown(KeyCode.Space) && lineNB >= EndingA.Length / 4)
            {
                Debug.Log("End of Intro");
                //We call function here to load scene
            }
        }
        else if (isEndingB)
        {
            if (startEnding)
            {
                startEnding = false;
                CheckAndSwapCinematic(lineNB);
                CheckAndPlaySound(lineNB);
                CheckAndPlaySoundtrack(lineNB);
                lineNB++;
            }
            if (Input.GetKeyDown(KeyCode.Space) && lineNB < EndingB.Length / 4)
            {
                //This statements blocks sounds, soundtracks or Cinematics from swapping until all text is build
                if (!architect1.isBuilding && !architect2.isBuilding && !architect3.isBuilding)
                {
                    CheckAndSwapCinematic(lineNB);
                    CheckAndPlaySound(lineNB);
                    CheckAndPlaySoundtrack(lineNB);
                }


                if (architect1.isBuilding || architect2.isBuilding || architect3.isBuilding)
                {
                    //Allows us to accelerate text typewriting or force it to complete depending on current dialogue box been used
                    if (architect1.isBuilding)
                    {
                        if (!architect1.fasterText)
                            architect1.fasterText = true;
                        else
                            architect1.ForceComplete();
                    }
                    else if (architect2.isBuilding)
                    {
                        if (!architect2.fasterText)
                            architect2.fasterText = true;
                        else
                            architect2.ForceComplete();
                    }
                    else if (architect3.isBuilding)
                    {
                        if (!architect3.fasterText)
                            architect3.fasterText = true;
                        else
                            architect3.ForceComplete();
                    }
                }
                else if (EndingB[lineNB, 1] == "1")
                {
                    architect1.Build(EndingB[lineNB, 2]);
                    CheckAndPlayVoiceline(lineNB, EndingB);
                    lineNB++;
                }
                else if (EndingB[lineNB, 1] == "2")
                {
                    architect2.Build(EndingB[lineNB, 2]);
                    CheckAndPlayVoiceline(lineNB, EndingB);
                    lineNB++;
                }
                else if (EndingB[lineNB, 1] == "3")
                {
                    architect3.Build(EndingB[lineNB, 2]);
                    CheckAndPlayVoiceline(lineNB, EndingB);
                    lineNB++;
                }
                else if (EndingB[lineNB, 1] == "0")
                {
                    architect1.Build(EndingB[lineNB, 2]);
                    architect2.Build(EndingB[lineNB, 2]);
                    architect3.Build(EndingB[lineNB, 2]);
                    CheckAndPlayVoiceline(lineNB, EndingB);
                    lineNB++;
                }
            }
            else if (Input.GetKeyDown(KeyCode.Space) && lineNB >= EndingB.Length / 4)
            {
                Debug.Log("End of Intro");
                //We call function here to load scene
            }
        }
        else if (isEndingC)
        {
            if (startEnding)
            {
                startEnding = false;
                CheckAndSwapCinematic(lineNB);
                CheckAndPlaySound(lineNB);
                CheckAndPlaySoundtrack(lineNB);
                lineNB++;
            }
            if (Input.GetKeyDown(KeyCode.Space) && lineNB < EndingC.Length / 4)
            {
                //This statements blocks sounds, soundtracks or Cinematics from swapping until all text is build
                if (!architect1.isBuilding && !architect2.isBuilding && !architect3.isBuilding)
                {
                    CheckAndSwapCinematic(lineNB);
                    CheckAndPlaySound(lineNB);
                    CheckAndPlaySoundtrack(lineNB);
                }


                if (architect1.isBuilding || architect2.isBuilding || architect3.isBuilding)
                {
                    //Allows us to accelerate text typewriting or force it to complete depending on current dialogue box been used
                    if (architect1.isBuilding)
                    {
                        if (!architect1.fasterText)
                            architect1.fasterText = true;
                        else
                            architect1.ForceComplete();
                    }
                    else if (architect2.isBuilding)
                    {
                        if (!architect2.fasterText)
                            architect2.fasterText = true;
                        else
                            architect2.ForceComplete();
                    }
                    else if (architect3.isBuilding)
                    {
                        if (!architect3.fasterText)
                            architect3.fasterText = true;
                        else
                            architect3.ForceComplete();
                    }
                }
                else if (EndingC[lineNB, 1] == "1")
                {
                    architect1.Build(EndingC[lineNB, 2]);
                    CheckAndPlayVoiceline(lineNB, EndingC);
                    lineNB++;
                }
                else if (EndingC[lineNB, 1] == "2")
                {
                    architect2.Build(EndingC[lineNB, 2]);
                    CheckAndPlayVoiceline(lineNB, EndingC);
                    lineNB++;
                }
                else if (EndingC[lineNB, 1] == "3")
                {
                    architect3.Build(EndingC[lineNB, 2]);
                    CheckAndPlayVoiceline(lineNB, EndingC);
                    lineNB++;
                }
                else if (EndingC[lineNB, 1] == "0")
                {
                    architect1.Build(EndingC[lineNB, 2]);
                    architect2.Build(EndingC[lineNB, 2]);
                    architect3.Build(EndingC[lineNB, 2]);
                    CheckAndPlayVoiceline(lineNB, EndingC);
                    lineNB++;
                }
            }
            else if (Input.GetKeyDown(KeyCode.Space) && lineNB >= EndingC.Length / 4)
            {
                Debug.Log("End of Intro");
                //We call function here to load scene
            }
        }
        FadeOutSoundtrack(endSoundtrack);
    }

    private void CheckAndSwapCinematic(int lnNB)
    {
        if (isEndingA)
        {
            switch (lnNB)
            {
                case 0:
                    CinematicImage.GetComponent<RawImage>().texture = EDImage1;
                    return;
                case 3:
                    CinematicImage.GetComponent<RawImage>().texture = EDImage2;
                    EDVoiceline.Stop();
                    return;
                case 7:
                    CinematicImage.GetComponent<RawImage>().texture = EDImage3;
                    EDVoiceline.Stop();
                    return;
                case 11:
                    CinematicImage.GetComponent<RawImage>().texture = EDImage4;
                    EDVoiceline.Stop();
                    return;
                case 19:
                    CinematicImage.SetActive(false);
                    return;
            }
        }

        if (isEndingB)
        {
            switch (lnNB)
            {
                case 0:
                    CinematicImage.GetComponent<RawImage>().texture = EDImage1;
                    return;
                case 3:
                    CinematicImage.GetComponent<RawImage>().texture = EDImage2;
                    EDVoiceline.Stop();
                    return;
                case 7:
                    CinematicImage.GetComponent<RawImage>().texture = EDImage3;
                    EDVoiceline.Stop();
                    return;
                case 11:
                    CinematicImage.GetComponent<RawImage>().texture = EDImage4;
                    EDVoiceline.Stop();
                    return;
                case 19:
                    CinematicImage.SetActive(false);
                    return;
            }
        }

        if (isEndingC)
        {
            switch (lnNB)
            {
                case 0:
                    CinematicImage.GetComponent<RawImage>().texture = EDImage1;
                    return;
                case 5:
                    CinematicImage.GetComponent<RawImage>().texture = EDImage2;
                    EDVoiceline.Stop();
                    return;
                case 9:
                    CinematicImage.GetComponent<RawImage>().texture = EDImage3;
                    EDVoiceline.Stop();
                    return;
                case 13:
                    CinematicImage.GetComponent<RawImage>().texture = EDImage4;
                    EDVoiceline.Stop();
                    return;
                case 21:
                    CinematicImage.SetActive(false);
                    return;
            }
        }

    }

    //This fucntion needs a list of 3 sounds
    private void CheckAndPlaySound(int lnNB)
    {
        if (EDSoundList.Length != 0)
        {
            switch (lnNB)
            {
                case 15:
                    EDSound.clip = EDSoundList[0];
                    EDSound.Play();
                    return;
                case 20:
                    EDSound.clip = EDSoundList[1];
                    EDSound.Play();
                    return;
                case 21:
                    EDSound.clip = EDSoundList[1];
                    EDSound.Play();
                    return;
                case 22:
                    EDSound.clip = EDSoundList[1];
                    EDSound.Play();
                    return;
                case 24:
                    EDSound.clip = EDSoundList[2];
                    EDSound.Play();
                    return;
            }
        }
    }

    //This function needs a list of 2 soundtracks
    private void CheckAndPlaySoundtrack(int lnNB)
    {
        if (EDSoundtrackList.Length != 0)
        {
            switch (lnNB)
            {
                case -1:
                    EDSoundtrack.clip = EDSoundtrackList[0];
                    EDSoundtrack.Play();
                    return;
                case 7:
                    endSoundtrack = true;
                    return;
                case 8:
                    EDSoundtrack.clip = EDSoundtrackList[1];
                    EDSoundtrack.Play();
                    return;
                case 15:
                    endSoundtrack = true;
                    return;
                case 16:
                    EDSoundtrack.clip = EDSoundtrackList[2];
                    EDSoundtrack.Play();
                    return;
                case 18:
                    EDSoundtrack.Stop();
                    return;
            }
        }
    }

    //This function needs a list of 16 voicelines
    private void CheckAndPlayVoiceline(int lnNB, string[,] ED)
    {
        if (ED[lnNB, 3] != "" && EDVoicelineList.Length != 0)
        {
            EDVoiceline.clip = EDVoicelineList[(int.Parse(ED[lnNB, 3]) - 1)];
            EDVoiceline.Play();
        }
    }

    private void FadeOutSoundtrack(bool fd)
    {
        if (fd)
        {
            if (EDSoundtrack.volume <= 0.1f)
            {
                EDSoundtrack.Stop();
                endSoundtrack = false;
            }
            else
            {
                float newVolume = EDSoundtrack.volume - (0.1f * Time.deltaTime);  //rate of the volume dropping
                if (newVolume < 0f)
                {
                    newVolume = 0f;
                }
                EDSoundtrack.volume = newVolume;
            }
        }
    }

    #region IntroText
    /// Here, we have lines that are made of 3 elements. 
    /// Element 1: Determines WHICH or IF character object is active
    /// Element 2: Determines what dialogue text box is used to write. If value = 0, it will select all of them (Basically, which architect of the ones created to use) <summary>
    /// Element 3: Contains the dialogue 
    /// Element 4: The number of the Voiceline to implement

    string[,] EndingA = new string[20, 4]
    {
        //1
        { "None", "1", "A female demon and a floating figure walk pass the mine carts and rails of a mine", "1" },
        //2
        { "None", "2", "Light comes through the roof and at the end of the tunnel, illuminating a legacy long forgotten, and marking a new beginning", "2" },
        //3
        { "None", "3", "The legacy of an old civilization, that once used those tools to cultivate its strength", "3" },
        //4
        { "None", "0", "", "" }, //lineNB 3 
        //5
        { "None", "1", "The last room was left behind, as our Wanderers reach the end of the tunnel", "4" },
        //6
        { "None", "2", "So many tried, so many failed", "5" },
        //7
        { "None", "3", "Yet, in their mind, there is always the question of what they will find at the end of the abyss?", "6" },
        //8
        { "None", "0", "", "" }, //lineNB 7
        //9
        { "None", "1", "The answer is obviously... Nature", "7" },
        //10
        { "None", "2", "For in the absence of humans, nature dominates and prevails", "8" },
        //11
        { "None", "3", "But the world is not so small, and sooner or later...", "9" },
        //12
        { "None", "0", "", "" }, //lineNB 11
        //13
        { "None", "2", "... The legacy of past or present civilizations will be found between the green vines of nature", "10" },
        //14
        { "None", "0", "", "" }, //lineNB 13
        //15
        { "None", "1", "As Chloe and Azure watch the ruins of a village close to Labyssal, they feel a sense of both relief and dissapointment for not having the chance to interact with other people", "11" },
        //16
        { "None", "2", "Yet, they don't feel discouraged since this is only the first discovery of many to come in their travels...", "12" },
        //17
        { "None", "3", " of The New World", "13" },
        //18
        { "None", "0", "", "" }, //lineNB 19
        //19
        { "None", "1", "Ending A:", "" },
        //20
        { "None", "2", "Wanderers of The New World", "" }
    };

    string[,] EndingB = new string[22, 4]
    {
        //1
        { "None", "1", "A female demon and a floating figure walk pass a mine and react the light at the end of the tunnel", "1" },
        //2
        { "None", "2", "At the end of their journey in Labyssal, many question have yet to be answered", "2" },
        //3
        { "None", "3", "What happened inside Labyssal? Who are are they and how do they ended up here?", "3" },
        //4
        { "None", "0", "", "" }, //lineNB 3 
        //5
        { "None", "1", "Perhaps those questions will never be answer", "4" },
        //6
        { "None", "2", "For those answers will remain lying in the abyss", "5" },
        //7
        { "None", "3", "While the girls go about accomplishing their visions and Dreams", "6" },
        //8
        { "None", "0", "", "" }, //lineNB 7
        //9
        { "None", "1", "From records inside Labyssal, they knew it existed out there", "7" },
        //10
        { "None", "2", "And at last, they found it...", "8" },
        //11
        { "None", "3", "Civilization", "9" },
        //12
        { "None", "0", "", "" }, //lineNB 11
        //13
        { "None", "1", "But this day is not to be the one to make contact", "10" },
        //14
        { "None", "2", "For they knew that both parties were not prepared to meet each other when taking into account the bad blood between all of them", "11" },
        //15
        { "None", "3", "But that day will come", "12" },
        //16
        { "None", "0", "", "" }, //lineNB 15
        //17
        { "None", "1", "Not now, perhaps not tomorrow, but it will", "13" },
        //18
        { "None", "2", "Preparations will need to be made", "14" },
        //19
        { "None", "3", "But at the moment, both girls will enjoy their earned spot Under the Sun", "15" },
        //20
        { "None", "0", "", "" }, //lineNB 19
        //21
        { "None", "1", "Ending B:", "" },
        //22
        { "None", "2", "Dreamers Under the Sun", "" }
    };

    string[,] EndingC = new string[24, 4]
    {
        //1
        { "None", "1", "A queen and a mother walk pass a mine and react the light at the end of the tunnel", "1" },
        //2
        { "None", "2", "Despite the truth, Rayne cannot help but feel proud of her daughter, her figure projecting determination and strength", "2" },
        //3
        { "None", "3", "When she entered the caves, Chloe was a little girl and slave", "3" },
        //4
        { "None", "0", "", "" }, //lineNB 3 
        //5
        { "None", "2", "Now she leaves as a woman and queen", "4" },
        //6
        { "None", "0", "", "" }, //lineNB 5
        //7
        { "None", "1", "Outside the caves, the legacy of the Black Wave shows the consequences of desire", "5" },
        //8
        { "None", "2", "For Labyssal... For Chloe, it is a reminder of how vengeance might go too far", "6" },
        //9
        { "None", "3", "As mother and daughter leave the abandoned village, they are reminded that, although forgotten, the legacy of Legends will always remain behind in some way", "7" },
        //10
        { "None", "0", "", "" }, //lineNB 9
        //11
        { "None", "1", "It did not take long for them to find civilization", "7" },
        //12
        { "None", "2", "Chloe was aware of the existence of this village, yet excitement and anxiety fills her at the though of approaching", "8" },
        //13
        { "None", "3", "A simple illusion spell from a Reborn and now powerful Chloe allows them to pass unnoticed", "9" },
        //14
        { "None", "0", "", "" }, //lineNB 13
        //15
        { "None", "1", "The day turns into a sunset, Chloe goes out to say a final goodbye", "10" },
        //16
        { "None", "2", "She will miss her home, but she will not look back at the past anymore", "11" },
        //17
        { "None", "3", "Now she has a task ahead of herself... A new commitment", "12" },
        //18
        { "None", "0", "", "" }, //lineNB 17
        //19
        { "None", "1", "But, if there is a day where she decides to come back, it should be for curiosity, instead of nostalgia", "13" },
        //20
        { "None", "2", "For her kingdom is vast, and not even the queen knows about all the treasures inside it", "14" },
        //21
        { "None", "3", "Treasures such as the knowledge that could be gained from its Memories", "15" },
        //22
        { "None", "0", "", "" }, //lineNB 21
        //23
        { "None", "1", "Ending C:", "" },
        //24
        { "None", "2", "Legends Reborn from Memories", "" }
    };

    #endregion
}

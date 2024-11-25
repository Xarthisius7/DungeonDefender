using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    DialogueSystem ds;
    TextArchitect architect1;
    TextArchitect architect2;
    TextArchitect architect3;

    public GameObject CharacterOne;
    public GameObject CharacterTwo;
    public GameObject CharacterThree;
    public GameObject CharacterFour;
    public GameObject CinematicImage;

    public Texture IntroImage1;
    public Texture IntroImage2;
    public Texture IntroImage3;
    public Texture IntroImage4;

    public AudioSource IntroSound;
    public AudioSource IntroSoundtrack;
    public AudioSource IntroVoiceline;

    public AudioClip[] IntroSoundList;
    public AudioClip[] IntroSoundtrackList;
    public AudioClip[] IntroVoicelineList;

    public TextArchitect.BuildMethod bm = TextArchitect.BuildMethod.instant;

    private bool startIntro;
    private bool endSoundtrack;
    private int lineNB;

    private string CharacterObjActive;

    void Start()
    {
        startIntro = true;
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

        //Makes all character objects invisible
        CharacterOne.SetActive(false);
        CharacterTwo.SetActive(false);
        CharacterThree.SetActive(false);
        CharacterFour.SetActive(false);




    }

    // Update is called once per frame
    void Update()
    {
        if (startIntro)
        {
            startIntro = false;
            CheckAndSwapCinematic(lineNB);
            CheckAndPlaySound(lineNB);
            CheckAndPlaySoundtrack(lineNB);
            lineNB++;
        }
        if (Input.GetKeyDown(KeyCode.Space) && lineNB < Intro.Length / 4)
        {
            //This statements blocks sounds, soundtracks or Cinematics from swapping until all text is build
            if (!architect1.isBuilding && !architect2.isBuilding && !architect3.isBuilding)
            {
                //Check if there is a need to swap character objects
                if (CharacterObjActive != Intro[lineNB, 0])
                {
                    CharacterObjActive = Intro[lineNB, 0];
                    SwapCharacterObject(CharacterObjActive);
                }

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
            else if (Intro[lineNB, 1] == "1")
            {
                architect1.Build(Intro[lineNB, 2]);
                CheckAndPlayVoiceline(lineNB);
                lineNB++;
            }
            else if (Intro[lineNB, 1] == "2")
            {
                architect2.Build(Intro[lineNB, 2]);
                CheckAndPlayVoiceline(lineNB);
                lineNB++;
            }
            else if (Intro[lineNB, 1] == "3")
            {
                architect3.Build(Intro[lineNB, 2]);
                CheckAndPlayVoiceline(lineNB);
                lineNB++;
            }
            else if (Intro[lineNB, 1] == "0")
            {
                architect1.Build(Intro[lineNB, 2]);
                architect2.Build(Intro[lineNB, 2]);
                architect3.Build(Intro[lineNB, 2]);
                CheckAndPlayVoiceline(lineNB);
                lineNB++;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space) && lineNB >= Intro.Length / 4)
        {
            Debug.Log("End of Intro");
            //We call function here to load scene
            SceneManager.LoadScene(3);
        }
        FadeOutSoundtrack(endSoundtrack);
    }

    private void SwapCharacterObject(string ch)
    {
        switch (ch)
        {
            case "None":
                CharacterOne.SetActive(false);
                CharacterTwo.SetActive(false);
                CharacterThree.SetActive(false);
                CharacterFour.SetActive(false);
                return;
            case "One":
                CharacterOne.SetActive(true);
                CharacterTwo.SetActive(false);
                CharacterThree.SetActive(false);
                CharacterFour.SetActive(false);
                return;
            case "Two":
                CharacterOne.SetActive(false);
                CharacterTwo.SetActive(true);
                CharacterThree.SetActive(false);
                CharacterFour.SetActive(false);
                return;
            case "Three":
                CharacterOne.SetActive(false);
                CharacterTwo.SetActive(false);
                CharacterThree.SetActive(true);
                CharacterFour.SetActive(false);
                return;
            case "Four":
                CharacterOne.SetActive(false);
                CharacterTwo.SetActive(false);
                CharacterThree.SetActive(false);
                CharacterFour.SetActive(true);
                return;
        }

    }

    private void CheckAndSwapCinematic(int lnNB)
    {
        switch (lnNB)
        {
            case 0:
                CinematicImage.GetComponent<RawImage>().texture = IntroImage1;
                return;
            case 3:
                CinematicImage.GetComponent<RawImage>().texture = IntroImage2;
                IntroVoiceline.Stop();
                return;
            case 7:
                CinematicImage.GetComponent<RawImage>().texture = IntroImage3;
                IntroVoiceline.Stop();
                return;
            case 11:
                CinematicImage.GetComponent<RawImage>().texture = IntroImage4;
                IntroVoiceline.Stop();
                return;
            case 19:
                CinematicImage.SetActive(false);
                return;
        }

    }

    //This fucntion needs a list of 3 sounds
    private void CheckAndPlaySound(int lnNB)
    {
        switch (lnNB)
        {
            case 15:
                IntroSound.clip = IntroSoundList[0];
                IntroSound.Play();
                return;
            case 20:
                IntroSound.clip = IntroSoundList[1];
                IntroSound.Play();
                return;
            case 21:
                IntroSound.clip = IntroSoundList[1];
                IntroSound.Play();
                return;
            case 22:
                IntroSound.clip = IntroSoundList[1];
                IntroSound.Play();
                return;
            case 24:
                IntroSound.clip = IntroSoundList[2];
                IntroSound.Play();
                return;
        }
    }

    //This function needs a list of 2 soundtracks
    private void CheckAndPlaySoundtrack(int lnNB)
    {
        switch (lnNB)
        {
            case -1:
                IntroSoundtrack.clip = IntroSoundtrackList[0];
                IntroSoundtrack.Play();
                return;
            case 7:
                endSoundtrack = true;
                return;
            case 8:
                IntroSoundtrack.clip = IntroSoundtrackList[1];
                IntroSoundtrack.Play();
                return;
            case 15:
                endSoundtrack = true;
                return;
            case 16:
                IntroSoundtrack.clip = IntroSoundtrackList[2];
                IntroSoundtrack.Play();
                return;
            case 18:
                IntroSoundtrack.Stop();
                return;
        }
    }

    //This function needs a list of 16 voicelines
    private void CheckAndPlayVoiceline(int lnNB)
    {
        if (Intro[lnNB, 3] != "")
        {
            IntroVoiceline.clip = IntroVoicelineList[(int.Parse(Intro[lnNB, 3]) - 1)];
            IntroVoiceline.Play();
        }
    }

    private void FadeOutSoundtrack(bool fd)
    {
        if (fd)
        {
            if (IntroSoundtrack.volume <= 0.1f)
            {
                IntroSoundtrack.Stop();
                endSoundtrack = false;
            }
            else
            {
                float newVolume = IntroSoundtrack.volume - (0.1f * Time.deltaTime);  //rate of the volume dropping
                if (newVolume < 0f)
                {
                    newVolume = 0f;
                }
                IntroSoundtrack.volume = newVolume;
            }
        }
    }

    #region IntroText
    /// Here, we have lines that are made of 3 elements. 
    /// Element 1: Determines WHICH or IF character object is active
    /// Element 2: Determines what dialogue text box is used to write. If value = 0, it will select all of them (Basically, which architect of the ones created to use) <summary>
    /// Element 3: Contains the dialogue 
    /// Element 4: The number of the Voiceline to implement

    string[,] Intro = new string[25, 4]
    {
        { "None", "1", "A female demon¡¯s body walks through an obscure cave, its steps echoing around the darkness", "1" },
        { "None", "2", "Despite the darkness, the body walks with purpose, aware of its surroundings", "2" },
        { "None", "3", "This is the case for this body and all others in this place, for they are all connect to Labyssal, the hivemind of the maze and caves", "3" },
        { "None", "0", "", "" }, //lineNB 3 
        { "None", "1", "The bodies, the plants, the insects, the floor, the walls, and even the darkness itself is part of the collective", "4" },
        { "None", "2", "Made of magic and being millennia old, the collective hivemind known as Labyssal feels and knows everything inside their domain", "5" },
        { "None", "3", "Yet it is only recently that it gained true sentience, and the ability to think clearly, feel, desire and HATE", "6" },
        { "None", "0", "", "" }, //lineNB 7
        { "None", "1", "The air is filled with hate and despair", "7" },
        { "None", "2", "After gaining sentience, their corruption and their hate towards life and individuality have only grown", "8" },
        { "None", "3", "All the adventurers that have tried to claim the vast treasures inside the caves and maze have perish, and have been assimilated by the collective", "9" },
        { "None", "0", "", "" }, //lineNB 11
        { "None", "1", "Suddenly, the female demon¡¯s body stops as through its eyes, Labyssal, the collective, sees a necklace with a perfect and pure blue gem at its center in the ground", "10" },
        { "None", "2", "For some reason, the collective is only able to see this artifact through the eyes of this body, and they are not able to detect or feel it by other means", "11" },
        { "None", "3", "Intrigued by this discovery, the body bends down and picks the artifac-", "12" },
        { "None", "0", "", "" }, //lineNB 15
        { "None", "1", "Pain...! Pain? Hurts? Hurts!!", "13" },
        { "None", "2", "The collective feels as a piece of their being is ripped out of them, Labyssal cries in pain and shock, the world trembles¡­¡­", "14" },
        { "None", "3", "then silence", "15" },
        { "None", "0", "", "" }, //lineNB 19
        { "One", "1", "...", "" },
        { "Two", "2", ".........", "" },
        { "Three", "3", "...............", "" },
        { "Three", "0", "", "" }, //lineNB 23
        { "Four", "2", "Chloe opens her eyes", "16" }
    };

    #endregion

}

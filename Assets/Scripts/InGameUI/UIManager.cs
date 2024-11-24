using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameController;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }


    [SerializeField] Image HealthBar;
    [SerializeField] Image StaminaBar;

    [SerializeField] float fillSpeed;


    public List<TextMeshProUGUI> attributeTexts; // Editable in the Unity Inspector, corresponding to 7 attributes
    public Image spellLogo; // Editable in the Unity Inspector for displaying spell logos

    public float spellDisplayXOffset = 5f; // Horizontal offset for displaying spells
    public float spellDisplayYOffset = 5f; // Vertical offset for displaying spells

    public GameObject PowerupMenuPanel; // The menu panel to activate/deactivate

    public TextMeshProUGUI[] subtitleTexts;
    public float displayTime = 2.5f;
    public float fadeDuration = 1f;

    private Queue<string> messageQueue = new Queue<string>(); //Ingame message display
    private Coroutine[] fadeCoroutines;


    public GameObject OpenItemMenu; // Menu object to show/hide
    public GameObject[] itemSlots; // Array of UI elements for the 3 item slots
    private List<int> offeredOptions = new List<int>(); // List to store the IDs of offered Powerups


    public TextMeshProUGUI waveText;

    public Image[] crystalImages; // crystal Defense progress 
    public Sprite activeCrystalSprite;

    public void UpdateCrystalsDisplay(int TowerDefensed)
    {
        for (int i = 0; i < crystalImages.Length; i++)
        {
            if (i < TowerDefensed)
            {
                crystalImages[i].sprite = activeCrystalSprite; 
                crystalImages[i].enabled = true;
            }
        }
    }

    public void ShowDefenseWaveRemain(int wave, int totalWave, float time)
    {
        //Show the wave status when player is defencing.
        StartCoroutine(UpdateWaveText(wave, totalWave, time));
    }

    private System.Collections.IEnumerator UpdateWaveText(int wave, int totalWave, float time)
    {
        while (time > 0)
        {
            waveText.text = $"Wave {wave}/{totalWave} Enemies are incoming... {Mathf.CeilToInt(time)}";
            yield return new WaitForSeconds(1f);
            time--;
        }
        waveText.text = "";
    }


    public void OpenItemMenuFunc(Powerup p1, Powerup p2, Powerup p3)
    {

        GameController.Instance.CloseAllMenu();
        EffectsManager.Instance.PlaySFX(13);
        // Store the Powerup data in an array for easy iteration
        Powerup[] powerups = { p1, p2, p3 };

        // Iterate through each Powerup and assign its data to the UI elements
        for (int i = 0; i < powerups.Length; i++)
        {
            Image icon = itemSlots[i].transform.Find("Icon").GetComponent<Image>();
            icon.sprite = powerups[i].sprite;

            TextMeshProUGUI description = itemSlots[i].transform.Find("Description").GetComponent<TextMeshProUGUI>();
            description.text = powerups[i].description;

            offeredOptions.Add(powerups[i].id);
        }

        // Set the menu object to active to make it visible
        OpenItemMenu.SetActive(true);
    }

    public void CloseItemMenu()
    {
        OpenItemMenu.SetActive(false);
    }

    public void popsUpDamage(float damage, Transform tf)
    {
        //TODO: show the damage player dult at given transform.
    }


    void Start()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        fadeCoroutines = new Coroutine[subtitleTexts.Length];
        ClearAllSubtitles();


    }
    public void ClickButton1()
    {
        ChooseOption(1);
    }
    public void ClickButton2()
    {
        ChooseOption(1);
    }
    public void ClickButton3()
    {
        ChooseOption(1);
    }

    public void ChooseOption(int i)
    {
        if (offeredOptions.Count == 0)
        {
            CloseItemMenu();
            return;
        }
        PowerupManager.instance.GivePowerup(offeredOptions[i]);
        offeredOptions.Clear();
        EffectsManager.Instance.PlaySFX(12);
        GameController.Instance.ResumeGame();
        CloseItemMenu();
    }

    public void ShowMessage(string message)
    {
        //add message to queue.
        messageQueue.Enqueue(message);
        if (messageQueue.Count > subtitleTexts.Length)
        {
            messageQueue.Dequeue();
        }
        UpdateSubtitles();
    }

    private void UpdateSubtitles()
    {
        for (int i = 0; i < fadeCoroutines.Length; i++)
        {
            if (fadeCoroutines[i] != null)
            {
                StopCoroutine(fadeCoroutines[i]);
            }
        }

        string[] messages = messageQueue.ToArray();
        ClearAllSubtitles();

        for (int i = 0; i < messages.Length; i++)
        {
            int index = subtitleTexts.Length - messages.Length + i;
            subtitleTexts[index].text = messages[i];
            fadeCoroutines[index] = StartCoroutine(FadeOutSubtitle(subtitleTexts[index], displayTime, fadeDuration));
        }
    }

    private IEnumerator FadeOutSubtitle(TextMeshProUGUI text, float delay, float duration)
    {
        //fade out the subtitle after a amount of time.
        yield return new WaitForSeconds(delay);

        float elapsedTime = 0f;
        Color originalColor = text.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            text.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        text.text = "";
        while (messageQueue.Count > 0)
        {
            messageQueue.Dequeue();
        }
        UpdateSubtitles();
    }

    private void ClearAllSubtitles()
    {
        foreach (TextMeshProUGUI text in subtitleTexts)
        {
            text.text = "";
            text.color = new Color(text.color.r, text.color.g, text.color.b, 1f);
        }
    }


    void Update()
    {

    }



    public void OpenAttrubuteMenu()
    {
        GameController.Instance.CloseAllMenu();
        // Update attribute values in the UI
        UpdateAttributesDisplay();

        // Display magic powerups
        DisplayMagicPowerups();

        // Set the menu panel active
        PowerupMenuPanel.SetActive(true);
    }
    public void CloseAttrubuteMenu()
    {
        PowerupMenuPanel.SetActive(false);
    }

    private void UpdateAttributesDisplay()
    {
        if (PowerupManager.instance != null)
        {
            // Update each attribute text
            attributeTexts[0].text = PowerupManager.instance.GetAttributeValue("MaxHealth").ToString();
            attributeTexts[1].text = PowerupManager.instance.GetAttributeValue("Attack").ToString();
            attributeTexts[2].text = PowerupManager.instance.GetAttributeValue("AttackSpeed").ToString();
            attributeTexts[3].text = PowerupManager.instance.GetAttributeValue("Defense").ToString();
            attributeTexts[4].text = PowerupManager.instance.GetAttributeValue("Speed").ToString();
            attributeTexts[5].text = PowerupManager.instance.GetAttributeValue("MaxStamina").ToString();
            attributeTexts[6].text = PowerupManager.instance.GetAttributeValue("StaminaRegenSpeed").ToString();
        }
    }

    // Method to display magic powerups with offsets
    private void DisplayMagicPowerups()
    {
        if (PowerupManager.instance != null)
        {
            List<Powerup> ownedPowerups = PowerupManager.instance.ownedPowerups;
            int spellCount = 0;

            foreach (Powerup powerup in ownedPowerups)
            {
                if (powerup.isMagic)
                {
                    // Instantiate a new spell logo
                    Image newSpellLogo = Instantiate(spellLogo, spellLogo.transform.parent);
                    newSpellLogo.sprite = powerup.sprite;
                    newSpellLogo.gameObject.SetActive(true);

                    // Set the position of the new spell logo
                    float xOffset = (spellCount % 3) * spellDisplayXOffset;
                    float yOffset = -(spellCount / 3) * spellDisplayYOffset;
                    newSpellLogo.rectTransform.anchoredPosition = spellLogo.rectTransform.anchoredPosition + new Vector2(xOffset, yOffset);

                    spellCount++;
                }
            }
        }
    }

    public void BroadCast(string message)
    {
        // broadCast a message on screen
        // each message will going up 1 tile after a new one appears, and fades out aftertime. 
        // can be called from any system.
    }

    public void ShowPauseScreen()
    {
        PauseMenu pauseMenu = FindAnyObjectByType<PauseMenu>();

        if (pauseMenu != null)
        {
            pauseMenu.ShowPauseScreen();
        }
        else
        {
            Debug.LogWarning("No pause screen Found");
        }
    }

    public void ClosePauseScreen()
    {
        PauseMenu pauseMenu = FindAnyObjectByType<PauseMenu>();

        if (pauseMenu != null)
        {
            pauseMenu.ClosePauseScreen();
        }
        else
        {
            Debug.LogWarning("No pause screen Found");
        }
    }


    public void ShowMainMenu()
    {

    }

    public void CloseMainMenu()
    {

    }

    public void UpdateHealth(float targetPercentage)
    {
        HealthBar.fillAmount = targetPercentage;
    }

    public void UpdateStamina(float percantange)
    {
        StaminaBar.fillAmount = percantange;
    }



}

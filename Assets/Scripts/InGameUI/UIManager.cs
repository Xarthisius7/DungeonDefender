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


    }

    void Update()
    {
        
    }
    public void OpenAttrubuteMenu()
    {
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

        if(pauseMenu != null)
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

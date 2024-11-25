using UnityEngine;
using UnityEngine.UI; // Use TMPro if TextMeshPro is used
using System.Collections;
using TMPro;

public class CreditsManager : MonoBehaviour
{
    private TextMeshProUGUI creditsText; // Replace with TMP_Text if using TextMeshPro
    public string fileName = "credits"; // Text file in Resources folder
    public float fadeDuration = 1.5f;
    public float displayDuration = 2f;

    private string[] creditEntries;

    void Start()
    {
        creditsText = GetComponent<TextMeshProUGUI>();

        LoadCredits();
        StartCoroutine(DisplayCredits());
    }

    void LoadCredits()
    {
        TextAsset creditsFile = Resources.Load<TextAsset>($"Menu/{fileName}");
        if (creditsFile != null)
        {
            creditEntries = creditsFile.text.Split(new string[] { "$$$" }, System.StringSplitOptions.RemoveEmptyEntries);
        }
        else
        {
            Debug.LogError("Credits file not found in Resources/Menu!");
        }
    }

    IEnumerator DisplayCredits()
    {
        foreach (var entry in creditEntries)
        {
            string processedText = entry;

            // Check if the text starts with "+=" and remove it
            if (processedText.StartsWith("+="))
            {
                processedText = processedText.Substring(2).Trim(); // Remove "+=" and any leading/trailing whitespace
                CenterText(); // Center the text horizontally and vertically
            }

            yield return StartCoroutine(FadeText(processedText, true)); // Fade In
            yield return new WaitForSeconds(displayDuration);          // Display
            yield return StartCoroutine(FadeText(processedText, false)); // Fade Out
        }

        SceneGameManager.Instance?.ReturnToMenu();
    }

    IEnumerator FadeText(string text, bool fadeIn)
    {
        creditsText.text = text;
        float elapsedTime = 0f;
        Color textColor = creditsText.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            textColor.a = fadeIn
                ? Mathf.Lerp(0, 1, elapsedTime / fadeDuration)
                : Mathf.Lerp(1, 0, elapsedTime / fadeDuration);

            creditsText.color = textColor;
            yield return null;
        }

        if (!fadeIn) creditsText.text = ""; // Clear text on fade-out
    }

    void CenterText()
    {
        creditsText.alignment = TextAlignmentOptions.Center; // Horizontally align text to the center
        creditsText.rectTransform.anchorMin = new Vector2(0.5f, 0.5f); // Anchor to center
        creditsText.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        creditsText.rectTransform.anchoredPosition = Vector2.zero; // Center the position
    }

    void ResetTextAlignment()
    {
        creditsText.alignment = TextAlignmentOptions.TopLeft; // Reset alignment to top-left or default
        creditsText.rectTransform.anchorMin = new Vector2(0, 1);
        creditsText.rectTransform.anchorMax = new Vector2(1, 1);
        creditsText.rectTransform.anchoredPosition = new Vector2(0, 0);
    }
}

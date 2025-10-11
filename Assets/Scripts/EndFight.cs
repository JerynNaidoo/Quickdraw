using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class EndFight : MonoBehaviour
{
    [Header("UI References")]
    public GameObject blackPanelObject;   // Panel GameObject
    public Image blackPanel;              // Optional: Image for fading
    public TextMeshProUGUI cinematicText; // Text for cinematic sequences
    public TextMeshProUGUI messageText;   // Optional: Text for ShowMessage/Hide

    [Header("Timing Settings")]
    public float panelFadeDuration = 1.2f;
    public float textFadeDuration = 1.2f;
    public float displayDuration = 2.0f;
    public float finalHoldDuration = 3.0f;

    [Header("Font Settings")]
    public float finalLineSizeMultiplier = 1.25f;

    [Header("Optional Scene Transition")]
    public bool autoLoadScene = false;
    public string sceneToLoad = "MainMenu";
    public float delayBeforeLoad = 4f;

    private float originalFontSize;

    private void Start()
    {
        if (cinematicText != null)
        {
            originalFontSize = cinematicText.fontSize;
            SetTextAlpha(0f);
            cinematicText.text = "";
        }

        if (blackPanel != null)
        {
            Color c = blackPanel.color;
            c.a = 0f;
            blackPanel.color = c;
            blackPanel.raycastTarget = false;
        }

        if (blackPanelObject != null)
        {
            blackPanelObject.SetActive(false);
        }
    }

    public IEnumerator PlayFinalSequence()
    {
        if (messageText != null)
            messageText.text = "";

        if (blackPanelObject != null)
            blackPanelObject.SetActive(true);


        Debug.Log("Final EndFight cinematic started...");

        if (blackPanel != null)
            yield return StartCoroutine(FadeImageAlpha(blackPanel, 0f, 1f, panelFadeDuration));

        yield return new WaitForSeconds(0.25f);

        string[] lines = new string[]
        {
            "You proved worthy of challenge.",
            "I have to take it upon my hands.",
            "Prepare yourself for the final battle!"
        };

        foreach (string line in lines)
        {
            yield return StartCoroutine(FadeInText(line, textFadeDuration));
            yield return new WaitForSeconds(displayDuration);
            yield return StartCoroutine(FadeOutText(textFadeDuration));
        }

        cinematicText.fontSize = originalFontSize * finalLineSizeMultiplier;
        yield return StartCoroutine(FadeInText("The final battle begins...", textFadeDuration));
        yield return new WaitForSeconds(finalHoldDuration);

        if (autoLoadScene && !string.IsNullOrEmpty(sceneToLoad))
        {
            yield return new WaitForSeconds(delayBeforeLoad);
            SceneManager.LoadScene(sceneToLoad);
        }

        if (blackPanelObject != null)
            blackPanelObject.SetActive(false);

    }

    private IEnumerator FadeImageAlpha(Image img, float from, float to, float duration)
    {
        if (img == null) yield break;

        float t = 0f;
        Color c = img.color;

        while (t < duration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(from, to, t / duration);
            img.color = new Color(c.r, c.g, c.b, a);
            yield return null;
        }

        img.color = new Color(c.r, c.g, c.b, to);
    }

    private IEnumerator FadeInText(string message, float duration)
    {
        if (cinematicText == null) yield break;

        cinematicText.text = message;
        SetTextAlpha(0f);
        float t = 0f;
        Color c = cinematicText.color;

        while (t < duration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, t / duration);
            cinematicText.color = c;
            yield return null;
        }

        c.a = 1f;
        cinematicText.color = c;
    }

    private IEnumerator FadeOutText(float duration)
    {
        if (cinematicText == null) yield break;

        float t = 0f;
        Color c = cinematicText.color;

        while (t < duration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, t / duration);
            cinematicText.color = c;
            yield return null;
        }

        c.a = 0f;
        cinematicText.color = c;
        cinematicText.text = "";
    }

    private void SetTextAlpha(float a)
    {
        if (cinematicText == null) return;

        Color c = cinematicText.color;
        c.a = a;
        cinematicText.color = c;
    }

    // ---------------- ShowMessage / Hide ----------------
    public void ShowMessage(string message)
    {
        if (blackPanelObject != null)
            blackPanelObject.SetActive(true);

        if (messageText != null)
            messageText.text = message;
    }

    public void Hide()
    {
        if (blackPanelObject != null)
            blackPanelObject.SetActive(false);

        if (messageText != null)
            messageText.text = "";

    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class EndGameCinematic : MonoBehaviour
{
    [Header("UI References")]
    public Image blackPanel;           // Full-screen black Image (alpha 0 at start)
    public TextMeshProUGUI endText;    // The TMP text (child of panel)

    [Header("Timing (seconds)")]
    public float panelFadeDuration = 1.2f;
    public float textFadeDuration = 1.2f;
    public float displayDuration = 2.0f;
    public float finalHoldDuration = 3.0f;

    [Header("Final line size")]
    public float finalLineSizeMultiplier = 1.25f;

    [Header("Optional scene transition")]
    public bool autoLoadScene = false;
    public string sceneToLoad = "MainMenu";
    public float delayBeforeLoad = 4f;

    [Header("Test")]
    public bool startOnStart = false;

    float originalFontSize;

    void Start()
    {
        blackPanel.raycastTarget = false; // make sure UI doesn't block

        if (blackPanel == null) Debug.LogError("blackPanel is not assigned!");
        if (endText == null) Debug.LogError("endText is not assigned!");
        // cache & initialise
        if (endText != null)
        {
            originalFontSize = endText.fontSize;
            SetTextAlpha(0f);
            endText.text = "";
        }

        if (blackPanel != null)
        {
            Color c = blackPanel.color;
            c.a = 0f;
            blackPanel.color = c;
            blackPanel.raycastTarget = false;
        }

        if (startOnStart) StartCoroutine(PlayEnding());
    }

    // Call this to start the cinematic (from boss script)
    public void StartCinematic()
    {
        StartCoroutine(PlayEnding());
    }

    IEnumerator PlayEnding()
    {
        // block input by enabling raycast target on the panel
        if (blackPanel != null) blackPanel.raycastTarget = true;

        // fade panel to black
        if (blackPanel != null)
            yield return StartCoroutine(FadeImageAlpha(blackPanel, 0f, 1f, panelFadeDuration));

        yield return new WaitForSeconds(0.25f);

        string[] lines = new string[]
        {
            "The dust settles…",
            "The outlaw falls.",
            "Peace returns to the town.",
            "Congratulations, Sheriff.",
            "You’ve beaten the final boss and completed the game."
        };

        foreach (string line in lines)
        {
            yield return StartCoroutine(FadeInText(line, textFadeDuration));
            yield return new WaitForSeconds(displayDuration);
            yield return StartCoroutine(FadeOutText(textFadeDuration));
        }

        // final "Thank you" larger line
        endText.fontSize = originalFontSize * finalLineSizeMultiplier;
        yield return StartCoroutine(FadeInText("Thank you for playing.", textFadeDuration));
        yield return new WaitForSeconds(finalHoldDuration);

        // optional scene transition
        if (autoLoadScene && !string.IsNullOrEmpty(sceneToLoad))
        {
            yield return new WaitForSeconds(delayBeforeLoad);
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    IEnumerator FadeImageAlpha(Image img, float from, float to, float duration)
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

    IEnumerator FadeInText(string message, float duration)
    {
        if (endText == null) yield break;
        endText.text = message;
        SetTextAlpha(0f);

        float t = 0f;
        Color c = endText.color;
        while (t < duration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, t / duration);
            endText.color = c;
            yield return null;
        }
        c.a = 1f;
        endText.color = c;
    }

    IEnumerator FadeOutText(float duration)
    {
        if (endText == null) yield break;
        float t = 0f;
        Color c = endText.color;
        while (t < duration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, t / duration);
            endText.color = c;
            yield return null;
        }
        c.a = 0f;
        endText.color = c;
        endText.text = "";
    }

    void SetTextAlpha(float a)
    {
        if (endText == null) return;
        Color c = endText.color;
        c.a = a;
        endText.color = c;
    }
}

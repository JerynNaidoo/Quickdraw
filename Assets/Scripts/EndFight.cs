using UnityEngine;
using TMPro;

public class EndFight : MonoBehaviour
{
    public GameObject blackPanel;       // assign BlackPanel GameObject
    public TextMeshProUGUI messageText; // assign MessageText

    void Start()
    {
        if (blackPanel != null)
            blackPanel.SetActive(false);
    }

    public void ShowMessage(string message)
    {
        if (blackPanel != null)
            blackPanel.SetActive(true);

        if (messageText != null)
            messageText.text = message;
    }

    public void Hide()
    {
        if (blackPanel != null)
            blackPanel.SetActive(false);
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class HealthBarManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject barPrefab;
    [SerializeField] private int maxBars = 10;
    public GameOverManager gameOverManager;
    [Header("Colors")]
    [SerializeField] private Color fullColor = new Color(0.11f, 0.73f, 0.28f); // Green
    [SerializeField] private Color midColor = Color.yellow;                     // Yellow
    [SerializeField] private Color lowColor = Color.red;                        // Red
    [SerializeField] private Color emptyColor = new Color(0.2f, 0.2f, 0.2f, 0.6f);

    [Header("Start")]
    [SerializeField, Range(0, 10)] private int startBars = 10;

    [Header("Animation")]
    [SerializeField] private float colorTransitionSpeed = 5f;
    [SerializeField] private float scalePopAmount = 1.2f;

    private List<Image> barImages = new List<Image>();
    private int currentBars;

    void Awake()
    {
        currentBars = Mathf.Clamp(startBars, 0, maxBars);
        BuildBars();
        UpdateBarsVisualInstant();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L)) RemoveOne();
        if (Input.GetKeyDown(KeyCode.P)) AddOne();
    }

    private void BuildBars()
    {
        if (barPrefab == null)
        {
            Debug.LogError("HealthBarManager: barPrefab is not assigned.");
            return;
        }

        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);

        barImages.Clear();

        for (int i = 0; i < maxBars; i++)
        {
            GameObject go = Instantiate(barPrefab, transform);
            go.transform.localScale = Vector3.one;
            Image img = go.GetComponent<Image>();
            if (img == null) img = go.AddComponent<Image>();
            barImages.Add(img);
        }
    }

    private void UpdateBarsVisualInstant()
    {
        for (int i = 0; i < barImages.Count; i++)
        {
            barImages[i].color = GetBarColor(i + 1);
            barImages[i].raycastTarget = false;
            barImages[i].transform.localScale = Vector3.one;
        }
    }

    private Color GetBarColor(int barIndex)
    {
        float ratio = (float)currentBars / maxBars;
        if (barIndex > currentBars) return emptyColor;
        if (ratio > 0.5f) return fullColor;
        if (ratio > 0.3f) return midColor;
        return lowColor;
    }

    private IEnumerator AnimateBar(Image img, Color targetColor)
    {
        Color startColor = img.color;
        Vector3 startScale = img.transform.localScale;
        Vector3 targetScale = Vector3.one;
        float timer = 0f;
        while (timer < 1f)
        {
            timer += Time.deltaTime * colorTransitionSpeed;
            img.color = Color.Lerp(startColor, targetColor, timer);
            img.transform.localScale = Vector3.Lerp(startScale, targetScale, timer);
            yield return null;
        }
        img.color = targetColor;
        img.transform.localScale = targetScale;
    }

    private void UpdateBarsVisual()
    {
        for (int i = 0; i < barImages.Count; i++)
        {
            Color targetColor = GetBarColor(i + 1);
            StopCoroutine("AnimateBar");
            StartCoroutine(AnimateBar(barImages[i], targetColor));
        }
        
    }

    public void AddOne()
    {
        if (currentBars >= maxBars) return;
        currentBars++;
        UpdateBarsVisual();
    }

    public void RemoveOne()
    {
        //if (currentBars <= 0) return;
        currentBars--;
        UpdateBarsVisual();

        if (currentBars <= 0)
        {
            Debug.Log("YOU DIED");
            if (gameOverManager != null)
                gameOverManager.Show();
        }
    }

    public void SetBars(int count)
    {
        currentBars = Mathf.Clamp(count, 0, maxBars);
        UpdateBarsVisual();
    }

    public int GetCurrentBars() => currentBars;

    public void ResetHealth()
    {
        currentBars = maxBars;
        UpdateBarsVisualInstant();  
    }



}

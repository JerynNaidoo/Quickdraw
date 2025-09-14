using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HealthBarManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject barPrefab;     // assign HealthBarSegment prefab
    [SerializeField] private int maxBars = 10;

    [Header("Colors")]
    [SerializeField] private Color filledColor = new Color(0.11f, 0.73f, 0.28f); // green
    [SerializeField] private Color emptyColor = new Color(0.2f, 0.2f, 0.2f, 0.6f); // grey-ish

    [Header("Start")]
    [SerializeField, Range(0, 10)] private int startBars = 10;

    private List<Image> barImages = new List<Image>();
    private int currentBars;

    void Awake()
    {
        currentBars = Mathf.Clamp(startBars, 0, maxBars);
        BuildBars();
        UpdateBarsVisual();
    }

    void Update()
    {
        // Test controls:
        if (Input.GetKeyDown(KeyCode.L)) RemoveOne();
        if (Input.GetKeyDown(KeyCode.P)) AddOne();
        if (Input.GetKeyDown(KeyCode.L)) Debug.Log("L pressed");
        if (Input.GetKeyDown(KeyCode.P)) Debug.Log("P pressed");
    }

    private void BuildBars()
    {
        if (barPrefab == null)
        {
            Debug.LogError("HealthBarManager: barPrefab is not assigned.");
            return;
        }

        // Remove old children (if any)
        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);

        barImages.Clear();

        for (int i = 0; i < maxBars; i++)
        {
            GameObject go = Instantiate(barPrefab, transform);
            go.transform.localScale = Vector3.one; // ensure scale is 1
            Image img = go.GetComponent<Image>();
            if (img == null) img = go.AddComponent<Image>();
            barImages.Add(img);
        }
    }

    private void UpdateBarsVisual()
    {
        for (int i = 0; i < barImages.Count; i++)
        {
            barImages[i].color = (i < currentBars) ? filledColor : emptyColor;
            barImages[i].raycastTarget = false; // don't block UI clicks
        }
    }

    // API:
    public void AddOne()
    {
        if (currentBars >= maxBars) return;
        currentBars++;
        UpdateBarsVisual();
    }

    public void RemoveOne()
    {
        if (currentBars <= 0) return;
        currentBars--;
        UpdateBarsVisual();
    }

    public void SetBars(int count)
    {
        currentBars = Mathf.Clamp(count, 0, maxBars);
        UpdateBarsVisual();
    }

    public int GetCurrentBars() => currentBars;
}

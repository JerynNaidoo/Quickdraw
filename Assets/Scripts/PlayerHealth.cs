using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int segmentValue = 10; // Each segment represents 10 HP
    private int currentHealth;

    [Header("UI")]
    public GameObject segmentPrefab; // Assign your prefab
    public Transform healthBarContainer; // Assign HealthBar object

    private List<Image> segments = new List<Image>();

    void Start()
    {
        currentHealth = maxHealth;
        CreateHealthSegments();
        UpdateHealthUI();
    }

    void CreateHealthSegments()
    {
        int numSegments = Mathf.CeilToInt((float)maxHealth / segmentValue);

        for (int i = 0; i < numSegments; i++)
        {
            GameObject segment = Instantiate(segmentPrefab, healthBarContainer);
            segments.Add(segment.GetComponent<Image>());
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();

        if (currentHealth <= 0)
            Die();
    }

    void UpdateHealthUI()
    {
        int fullSegments = currentHealth / segmentValue;
        int partialHealth = currentHealth % segmentValue;

        for (int i = 0; i < segments.Count; i++)
        {
            if (i < fullSegments)
                segments[i].fillAmount = 1f; // full segment
            else if (i == fullSegments)
                segments[i].fillAmount = (float)partialHealth / segmentValue; // partially filled
            else
                segments[i].fillAmount = 0f; // empty
        }
    }

    void Die()
    {
        Debug.Log("Player Died!");
    }
}

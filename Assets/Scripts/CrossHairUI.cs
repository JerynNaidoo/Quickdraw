using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CrossHairUI : MonoBehaviour
{
    [Header("Crosshair Lines (non-diagonal)")]
    [SerializeField] private RectTransform[] crosshairLines; // 4 main lines
    [SerializeField] private float expandDistance = 100f;
    [SerializeField] private float expandSpeed = 80f;
    [SerializeField] private float returnSpeed = 50f;

    [Header("Hitmarker (diagonal lines)")]
    [SerializeField] private RawImage[] hitmarkerLines;
    [SerializeField] private float hitmarkerDisplayTime = 0.15f;
    [SerializeField] private float hitmarkerFadeSpeed = 10f;

    private Vector2[] originalCrosshairPositions;
    private bool isExpanding = false;

    private void Start()
    {
        // Store crosshair line original positions
        if (crosshairLines != null && crosshairLines.Length > 0)
        {
            originalCrosshairPositions = new Vector2[crosshairLines.Length];
            for (int i = 0; i < crosshairLines.Length; i++)
                originalCrosshairPositions[i] = crosshairLines[i].anchoredPosition;
        }

        // Ensure hitmarkers start invisible
        if (hitmarkerLines != null && hitmarkerLines.Length > 0)
            SetHitmarkerAlpha(0f);
    }

    // Called when firing weapon
    public void AnimateCrosshair()
    {
        if (!isExpanding)
            StartCoroutine(ExpandAndReturn());
    }

    // Called when hitting an enemy
    public void ShowHitmarker()
    {
        StopCoroutine(nameof(FlashHitmarker));
        StartCoroutine(FlashHitmarker());
    }

    private IEnumerator ExpandAndReturn()
    {
        isExpanding = true;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * expandSpeed;
            for (int i = 0; i < crosshairLines.Length; i++)
            {
                // Move each line further away from its original position
                crosshairLines[i].anchoredPosition =
                    Vector2.Lerp(originalCrosshairPositions[i],
                                 originalCrosshairPositions[i] * (1 + expandDistance / 100f), t);
            }
            yield return null;
        }

        // Return smoothly
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * returnSpeed;
            for (int i = 0; i < crosshairLines.Length; i++)
            {
                crosshairLines[i].anchoredPosition =
                    Vector2.Lerp(crosshairLines[i].anchoredPosition,
                                 originalCrosshairPositions[i], t);
            }
            yield return null;
        }

        // Reset to exact position
        for (int i = 0; i < crosshairLines.Length; i++)
            crosshairLines[i].anchoredPosition = originalCrosshairPositions[i];

        isExpanding = false;
    }

    private IEnumerator FlashHitmarker()
    {
        SetHitmarkerAlpha(1f);
        yield return new WaitForSeconds(hitmarkerDisplayTime);

        float alpha = 1f;
        while (alpha > 0f)
        {
            alpha -= Time.deltaTime * hitmarkerFadeSpeed;
            SetHitmarkerAlpha(alpha);
            yield return null;
        }
        SetHitmarkerAlpha(0f);
    }

    private void SetHitmarkerAlpha(float alpha)
    {
        foreach (var img in hitmarkerLines)
        {
            if (img == null) continue;
            Color c = img.color;
            c.a = alpha;
            img.color = c;
        }
    }
}

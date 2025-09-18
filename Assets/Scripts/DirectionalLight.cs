using UnityEngine;

public class SimpleLightControl : MonoBehaviour
{
    public Material skyMaterial; 
    public Light sceneLight;     

    void Update()
    {
        
        float timeOfDay = skyMaterial.GetFloat("_TimeOfDay");

        
        sceneLight.intensity = Mathf.Lerp(0.9f, 1.0f, timeOfDay);

       
        sceneLight.color = Color.Lerp(
            new Color(0.95f, 0.95f, 0.9f), // Night colour
            new Color(1f, 0.95f, 0.9f),   // Day colour
            timeOfDay
        );
    }
}
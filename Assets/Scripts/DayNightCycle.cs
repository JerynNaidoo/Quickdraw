using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public Material skyMat;        
    public float cycleLength = 120f; //2mins

    void Update()
    {
        
        float t = (Time.time % cycleLength) / cycleLength;
        skyMat.SetFloat("_TimeOfDay", t);
    }
}

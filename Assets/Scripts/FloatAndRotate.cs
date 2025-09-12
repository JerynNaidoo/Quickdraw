using UnityEngine;

public class FloatAndRotate : MonoBehaviour
{
    public float floatStrength = 0.5f;
    public float rotationSpeed = 2f;
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    
    void Update()
    {
        
        transform.position = startPos + new Vector3(0, Mathf.Sin(Time.time) * floatStrength, 0);
        
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}

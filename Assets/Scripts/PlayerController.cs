using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    private Rigidbody rb;
    private float movementX;
    private float movementY;

    [Header("Movement Settings")]
    public float speed = 5f;

    [Header("Look Settings")]
    public float lookSensitivity = 2f;
    public float maxLookX = 80f;  // up/down clamp
    public float minLookX = -80f;

    private float rotationX;
    private float rotationY;

    private Camera playerCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        playerCamera = GetComponentInChildren<Camera>();
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);
        Vector3 worldMovement = transform.TransformDirection(movement); // move relative to facing direction
        rb.AddForce(worldMovement * speed);
    }
    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }
    void OnLook(InputValue lookValue)
    {
        Vector2 look = lookValue.Get<Vector2>();

        // horizontal (yaw) -> rotate player
        transform.Rotate(Vector3.up * look.x * lookSensitivity);

        // vertical (pitch) -> rotate camera only
        rotationX -= look.y * lookSensitivity;
        rotationX = Mathf.Clamp(rotationX, minLookX, maxLookX);
    
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
    }
}
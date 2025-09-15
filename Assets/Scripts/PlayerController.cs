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
    public float lookSensitivity = 0.1f;
    public float maxLookX = 80f;  // up/down clamp
    public float minLookX = -80f;

    private float rotationX;
    private float rotationY;

    private Camera playerCamera;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void FixedUpdate()
    {
        // get the forward and right vectors of the camera
        Transform cam = Camera.main.transform;

        Vector3 forward = cam.forward;
        Vector3 right = cam.right;

        // ignore vertical y-component
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        // get movement direction based on camera direction
        Vector3 moveDirection = forward * movementY + right * movementX;

        rb.AddForce(moveDirection * speed, ForceMode.Force);
    }
    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }
}
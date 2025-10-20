using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private float movementX;
    private float movementY;

    [Header("Movement Settings")]
    public float speed = 5f;
    public float gravity = -9.81f;
    //public float jumpHeight = 2f;

    [Header("Look Settings")]
    public float lookSensitivity = 2f;
    public float maxLookX = 45f;   // look up limit
    public float minLookX = -45f;  // look down limit

    private float rotationX;
    private float rotationY;

    private Vector3 velocity;   // for gravity
    private Camera playerCamera;

    public GameObject playerModel;
    public Transform head;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        playerCamera = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Load sensitivity setting
        lookSensitivity = PlayerPrefs.GetFloat("lookSensitivity", 0.5f);
    }

    public void UpdateSensitivity(float newSensitivity)
    {
        lookSensitivity = newSensitivity;
    }

    private void Update()
    {
        if (!PauseMenu.isPaused)
        {
            HandleMovement();
            HandleLook();
        }
    }

    void HandleMovement()
    {
        // Get the camera's forward & right vectors
        Transform cam = playerCamera.transform;
        Vector3 forward = cam.forward;
        Vector3 right = cam.right;

        // Ignore vertical influence
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        // Movement based on input
        Vector3 moveDirection = forward * movementY + right * movementX;

        // Apply movement
        controller.Move(moveDirection * speed * Time.deltaTime);

        // Apply gravity manually
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // small downward force to keep grounded
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleLook()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue() * lookSensitivity;

        rotationX -= mouseDelta.y;
        rotationY += mouseDelta.x;

        rotationX = Mathf.Clamp(rotationX, minLookX, maxLookX);

        // Vertical rotation for head (pitch)
        if (head != null)
            head.localRotation = Quaternion.Euler(rotationX, 0f, 0f);

        // Horizontal rotation for player body (yaw)
        transform.rotation = Quaternion.Euler(0f, rotationY, 0f);

        // Sync player model with body rotation
        if (playerModel != null)
            playerModel.transform.rotation = Quaternion.Euler(0f, rotationY, 0f);
    }


    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }
}
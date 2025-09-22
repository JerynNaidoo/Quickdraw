//using UnityEngine;
//using UnityEngine.InputSystem;

//public class PlayerController : MonoBehaviour
//{
//    private Rigidbody rb;
//    private float movementX;
//    private float movementY;

//    [Header("Movement Settings")]
//    public float speed = 5f;

//[Header("Look Settings")]
//    public float lookSensitivity = 0.1f;
//    public float maxLookX = 80f;  // up/down clamp
//    public float minLookX = -80f;

//    private float rotationX;
//    private float rotationY;

//    private Camera playerCamera;

//    void Start()
//    {
//        rb = GetComponent<Rigidbody>();
//        rb.freezeRotation = true;

//        Cursor.lockState = CursorLockMode.Locked;
//        Cursor.visible = false;
//    }

//    private void FixedUpdate()
//    {
//        // get the forward and right vectors of the camera
//        Transform cam = Camera.main.transform;

//        Vector3 forward = cam.forward;
//        Vector3 right = cam.right;

//        // ignore vertical y-component
//        forward.y = 0f;
//        right.y = 0f;
//        forward.Normalize();
//        right.Normalize();

//        // get movement direction based on camera direction
//        Vector3 moveDirection = forward * movementY + right * movementX;

//        // Preserve vertical (y) velocity for gravity & collisions
//        Vector3 velocity = moveDirection * speed;
//        velocity.y = rb.velocity.y;

//        rb.velocity = velocity;
//    }
//    void OnMove(InputValue movementValue)
//    {
//        Vector2 movementVector = movementValue.Get<Vector2>();
//        movementX = movementVector.x;
//        movementY = movementVector.y;
//    }
//}

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
    public float jumpHeight = 2f;

    [Header("Look Settings")]
    public float lookSensitivity = 2f;
    public float maxLookX = 80f;   // look up limit
    public float minLookX = -80f;  // look down limit

    private float rotationX;
    private float rotationY;

    private Vector3 velocity;   // for gravity
    private Camera playerCamera;

    public GameObject playerModel;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        playerCamera = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleMovement();
        HandleLook();
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
        // Mouse input
        Vector2 mouseDelta = Mouse.current.delta.ReadValue() * lookSensitivity;
        rotationX -= mouseDelta.y;
        rotationY += mouseDelta.x;
        // Clamp vertical rotation
        rotationX = Mathf.Clamp(rotationX, minLookX, maxLookX);

        // Create the camera rotation
        Quaternion cameraRotation = Quaternion.Euler(rotationX, 0f, 0f);

        // Apply to both camera and arms
        playerCamera.transform.localRotation = cameraRotation;
        playerModel.transform.localRotation = cameraRotation; // Add this

        // Rotate player left/right
        transform.rotation = Quaternion.Euler(0f, rotationY, 0f);
    }

    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }
}
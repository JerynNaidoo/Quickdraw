using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioSource pistolFire;       // Sound effect when gun fires
    [SerializeField] private GameObject pistol;
    [SerializeField] private Camera playerCam;

    [Header("Weapon Settings")]
    [SerializeField] private InputActionReference fireAction; // Input reference for shooting
    [SerializeField] private float damage = 50f;
    [SerializeField] private float range = 100f;
    [SerializeField] private float fireRate = 0.5f;          // Time between shots (must not be below 0.33f, shoot animation is 0.33s)

    private float nextTimeToFire = 0f; // Keeps track of cooldown between shots

    private Animator pistolAnimator; // animator component, will be used to check animation states

    private void Awake()
    {
        pistolAnimator = pistol.GetComponent<Animator>();
    }

    private void OnEnable()
    {
        fireAction.action.performed += OnFire;
        fireAction.action.Enable();
    }

    private void OnDisable()
    {
        fireAction.action.performed -= OnFire;
        fireAction.action.Disable();
    }

    private void OnFire(InputAction.CallbackContext context)
    {
        // Fire Rate limiting, can  only shoot if gun is idle and shot cooldown has passed
        if (Time.time >= nextTimeToFire && IsIdle())
        {
            nextTimeToFire = Time.time + fireRate;
            Shoot();
        }
    }

    private bool IsIdle()
    {
        AnimatorStateInfo stateInfo = pistolAnimator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName("Idle");
    }

    private void Shoot()
    {
        // Play firing sound
        pistolFire.Play();
        pistolAnimator.Play("PistolFire");

        // Raycast forward from camera
        RaycastHit hit;
        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, range))
        {
            Debug.Log("Hit: " + hit.transform.name);
            // Check if enemy was hit
            Enemy enemy = hit.transform.GetComponentInParent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }
}

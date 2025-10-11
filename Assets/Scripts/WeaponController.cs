using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioSource pistolFire;       // Sound effect when gun fires
    [SerializeField] private GameObject pistol;
    [SerializeField] private Camera playerCam;
    [SerializeField] private Revolver revolver;          // Reference to revolver script to check ammo

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
            if (revolver != null && revolver.CanShoot())
            {
                nextTimeToFire = Time.time + fireRate;
                Shoot();
                revolver.ConsumeAmmo();
            }
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

            // Try to get Enemy script from hit object or any parent
            Enemy enemy = hit.transform.GetComponent<Enemy>();
            Transform current = hit.transform;
            while (enemy == null && current.parent != null)
            {
                current = current.parent;
                enemy = current.GetComponent<Enemy>();
            }

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            else
            {
                // If not an Enemy, try Boss script
                current = hit.transform;
                Boss boss = hit.transform.GetComponent<Boss>();
                while (boss == null && current.parent != null)
                {
                    current = current.parent;
                    boss = current.GetComponent<Boss>();
                }

                if (boss != null)
                {
                    boss.TakeDamage(damage);
                }
            }
        }
    }

    


}

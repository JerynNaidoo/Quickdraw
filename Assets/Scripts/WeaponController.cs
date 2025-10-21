using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioSource pistolFire;
    [SerializeField] private GameObject player;// Sound effect when gun fires
    [SerializeField] private GameObject pistol;
    [SerializeField] private Camera playerCam;
    [SerializeField] private Revolver revolver;          // Reference to revolver script to check ammo
    [SerializeField] private CrossHairUI crosshairUI;

    [Header("Weapon Settings")]
    [SerializeField] private InputActionReference fireAction; // Input reference for shooting
    [SerializeField] private float damage = 50f;
    [SerializeField] private float range = 100f;
    [SerializeField] private float fireRate = 0.5f;          // Time between shots (must not be below 0.33f, shoot animation is 0.33s)

    private float nextTimeToFire = 0f; // Keeps track of cooldown between shots
    [SerializeField] private GameObject bloodEffectPrefab;
    private Animator playerAnimator; // animator component, will be used to check animation states

    private void Awake()
    {
        playerAnimator = player.GetComponent<Animator>();
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
        if (Time.time >= nextTimeToFire && IsIdle() && !PauseMenu.isPaused)
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
        AnimatorStateInfo stateInfo = playerAnimator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName("Idle");
    }

    private void Shoot()
    {
        // Play firing sound and animation
        playerAnimator.Play("ShootRevolver_1");
        // Play firing sound
        pistolFire.Play();

        // Raycast forward from the camera
        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out RaycastHit hit, range))
        {
            Debug.Log("Hit: " + hit.transform.name);

            if (bloodEffectPrefab != null &&
                (hit.collider.CompareTag("Enemy") || hit.collider.CompareTag("Boss")))
            {
                GameObject blood = Instantiate(
                    bloodEffectPrefab,
                    hit.point + hit.normal * 0.05f,
                    Quaternion.LookRotation(hit.normal)
                );
                blood.layer = LayerMask.NameToLayer("Ignore Raycast");
                Destroy(blood, 2f);

            }

            // After shooting
            crosshairUI?.AnimateCrosshair();

            // --- Damage Enemy ---
            Enemy enemy = hit.transform.GetComponentInParent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                crosshairUI?.ShowHitmarker();
                return;
            }

            // --- Damage Boss ---
            Boss boss = hit.transform.GetComponentInParent<Boss>();
            if (boss != null)
            {
                boss.TakeDamage(damage);
                crosshairUI?.ShowHitmarker();
            }
        }
    }


    public float GetDamage() => damage;
    public void SetDamage(float newDamage) => damage = newDamage;


}
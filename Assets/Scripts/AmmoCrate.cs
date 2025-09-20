using UnityEngine;

public class AmmoCrate : MonoBehaviour
{
    [Header("Ammo Settings")]
    public int ammoAmount = 12;             // how much reserve ammo the crate gives
    public Revolver revolver;               // drag the player's revolver here in Inspector
    public GameObject uiPrompt;

    private bool playerInRange = false;

    void Start()
    {
        if (uiPrompt != null)
            uiPrompt.SetActive(false); // hide initially
    }
    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (revolver != null)
            {
                revolver.AddReserveAmmo(ammoAmount);
                Debug.Log("Ammo crate used. Added " + ammoAmount + " reserve ammo.");

                
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (uiPrompt != null)
                uiPrompt.SetActive(true);  // show prompt
            Debug.Log("Player entered crate zone. Press E to pick up ammo.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (uiPrompt != null)
                uiPrompt.SetActive(false); // hide prompt
            Debug.Log("Player left crate zone.");
        }
    }
}

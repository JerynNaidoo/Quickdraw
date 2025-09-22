using UnityEngine;

public class Boss : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    [Header("Death Settings")]
    [SerializeField] private float despawnTime = 2f;
    private bool isDead = false;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    /// <summary>
    /// Call this to apply damage to the enemy.
    /// </summary>
    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage. Remaining HP: {currentHealth}");

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        Debug.Log($"{gameObject.name} died!");

        // TODO: Play animation, particles, sound here if needed

        Destroy(gameObject, despawnTime);
    }
}

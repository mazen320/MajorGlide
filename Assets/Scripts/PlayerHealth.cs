using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;
    public Slider healthBarSlider;
    public float healthBarAnimSpeed;
    private bool animateHealthBar;
    private float currentVelocity = 0f;

    public GameObject ragdollObject;

    private void Start()
    {
        currentHealth = maxHealth;
        healthBarSlider.maxValue = maxHealth;
        healthBarSlider.value = maxHealth;
    }

    private void Update()
    {
        // animate health bar
        if (animateHealthBar && healthBarSlider.value != currentHealth)
        {
            healthBarSlider.value = Mathf.SmoothDamp(healthBarSlider.value, currentHealth, ref currentVelocity, healthBarAnimSpeed * Time.deltaTime);
        }
        else if (healthBarSlider.value == currentHealth)
        {
            animateHealthBar = true;
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        animateHealthBar = true;

        if (currentHealth <= 0)
        {
            // Disable the player's original model
            gameObject.SetActive(false);

            // Enable the ragdoll object
            ragdollObject.SetActive(true);
        }
    }
}

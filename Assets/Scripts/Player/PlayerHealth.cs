using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public bool isDead;
    public int CurrentHealth{ get => currentHealth; private set => currentHealth = value; }

    [Header("Health Bar")]
    public int maxHealth;
    public float chipSpeed = 2f;
    public Image frontHealthBar;
    public Image backHealthBar;
    public TextMeshProUGUI healthText;

    [Header("Invincible")]
    public float invincibilityDuration;
    float invincibilityTimer;
    bool isInvincible;

    [Header("Damage Overlay")]
    public Image overlay;
    public float duration = 2f;
    public float fadeSpeed = 1.5f;

    int currentHealth;
    float lerpTimer;
    float durationTimer;

    void Awake()
    {
        currentHealth = maxHealth;
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 0f);
    }

    void Update()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if(invincibilityTimer > 0){
            invincibilityTimer -= Time.deltaTime;
        }else if(isInvincible){
            isInvincible = false;
        }

        UpdateHealthUI();

        if(overlay.color.a > 0){
            if(currentHealth < 30){ return; }
            
            durationTimer += Time.deltaTime;

            if(durationTimer > duration){
                float tempAlpha = overlay.color.a;
                tempAlpha -= Time.deltaTime;
                overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, tempAlpha);
            }
        }
    }

    public void UpdateHealthUI()
    {
        float fillFront = frontHealthBar.fillAmount;
        float fillBack = backHealthBar.fillAmount;
        float hFraction = currentHealth / maxHealth;

        // Take Damage
        if(fillBack > hFraction){
            frontHealthBar.fillAmount = hFraction;
            backHealthBar.color = Color.red;
            lerpTimer += Time.deltaTime;

            float percentComplete = lerpTimer / chipSpeed;
            percentComplete *= percentComplete;
            backHealthBar.fillAmount = Mathf.Lerp(fillBack, hFraction, percentComplete);
        }

        // Restore Health
        if(fillFront < hFraction){
            backHealthBar.color = Color.green;
            backHealthBar.fillAmount = hFraction;
            lerpTimer += Time.deltaTime;

            float percentComplete = lerpTimer / chipSpeed;
            percentComplete *= percentComplete;
            frontHealthBar.fillAmount = Mathf.Lerp(fillFront, backHealthBar.fillAmount, percentComplete);
        }

        healthText.text = $"{currentHealth}/{maxHealth}";
    }

    public void TakeDamage(int damageAmount)
    {
        if(!isInvincible){
            currentHealth -= Random.Range(damageAmount - 2, damageAmount);

            invincibilityTimer = invincibilityDuration;
            isInvincible = true;

            lerpTimer = 0f;
            duration = 0;
            overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 1f);

            if(currentHealth <= 0){ Death(); }
        }
    }

    public void RestoreHealth(int healAmount)
    {
        if(currentHealth < maxHealth){
            currentHealth += healAmount;

            if(currentHealth > maxHealth){
                currentHealth = maxHealth;
            }

            lerpTimer = 0f;
        }
    }

    public void Death()
    {
        SceneManager.LoadScene(0);
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
/// <summary>
/// ¿Œ∞‘¿” HUD
/// </summary>
public class InGameHUDController : MonoBehaviour
{
    [Header("Mech Health")]
    [SerializeField] private MechHull2D mechHull = null;
    [FormerlySerializedAs("healthfill")]
    [SerializeField] private Image healthFill = null;
    [SerializeField] private TMP_Text healthText = null;
    [SerializeField] private float maxHealthForDisplay = 500f;

    [Header("Health Animation")]
    [SerializeField] private float healthBarSpeed = 1.5f;

    private float displayedHealthRatio = 1f;
    private bool isHealthRationInitialized = false;

    public void SetMechHull(MechHull2D inMechHull)
    {
        mechHull = inMechHull;
        isHealthRationInitialized = false;
        UpdateHealthUI();
    }

    private void Update()
    {
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (mechHull == null || healthFill == null || healthText == null)
        {
            return;
        }

        float currentHealth = mechHull.Health;
        float maxHealth = Mathf.Max(1f, maxHealthForDisplay);
        float targetHealthRatio = Mathf.Clamp01(currentHealth / maxHealth);

        if (!isHealthRationInitialized)
        {
            displayedHealthRatio = targetHealthRatio;
            isHealthRationInitialized = true;
        }

        displayedHealthRatio = Mathf.MoveTowards(
            displayedHealthRatio,
            targetHealthRatio,
            healthBarSpeed * Time.deltaTime);

        healthFill.fillAmount = displayedHealthRatio;

        healthText.text = "HP : "
            + currentHealth.ToString("0.0")
            + " / "
            + maxHealth.ToString("0.0");
    }
}

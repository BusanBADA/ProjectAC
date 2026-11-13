using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class InGameHUDController : MonoBehaviour
{
    [Header("Mech Health")]
    [SerializeField] private MechHull2D mechHull = null;
    [FormerlySerializedAs("healthfill")]
    [SerializeField] private Image healthFill = null;
    [SerializeField] private TMP_Text healthText = null;
    [SerializeField] private float maxHealthForDisplay = 500f;

    public void SetMechHull(MechHull2D inMechHull)
    {
        mechHull = inMechHull;
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
        float maxHelth = Mathf.Max(1f, maxHealthForDisplay);
        float healthRatio = Mathf.Clamp01(currentHealth / maxHealthForDisplay);

        healthFill.fillAmount = healthRatio;

        healthText.text = "HP : "
            + currentHealth.ToString("0.0")
            + " / "
            + maxHelth.ToString("0.0");
    }
}

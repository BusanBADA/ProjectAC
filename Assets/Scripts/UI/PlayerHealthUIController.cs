using System.Collections;
using UnityEngine;

public class PlayerHealthUIController : MonoBehaviour
{
    [Header("Health UI Settings")]
    [SerializeField] private GameObject playerHealthUI = null;
    [SerializeField] private float displayDuration = 5f;

    private Coroutine hideCoroutine = null;

    private void Start()
    {
        if (playerHealthUI != null)
        {
            playerHealthUI.SetActive(false);
        }
    }

    public void ShowHealthUI()
    {
        if (playerHealthUI == null)
        {
            return;
        }

        playerHealthUI.SetActive(true);

        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }

        hideCoroutine = StartCoroutine(HideHealthUIAfterDelay());
    }

    private IEnumerator HideHealthUIAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        
        playerHealthUI.SetActive(false);
        hideCoroutine = null;
    }
}

using System.Collections;
using TMPro;
using UnityEngine;

public class GameInfoUIController : MonoBehaviour
{
    public void ShowMapName(string inMapName)
    {
        if (background == null || mapNameText == null)
        {
            return;
        }

        mapNameText.text = inMapName;

        background.SetActive(true);
        mapNameText.gameObject.SetActive(true);

        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }

        hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    [Header("Game Info UI")]
    [SerializeField] private GameObject background = null;
    [SerializeField] private TMP_Text mapNameText = null;

    [Header("Display Settings")]
    [SerializeField] private float displayDuration = 2.5f;

    private Coroutine hideCoroutine = null;

    private void Awake()
    {
        if (background != null)
        {
            background.SetActive(false);
        }

        if (mapNameText != null)
        {
            mapNameText.gameObject.SetActive(false);
        }
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);

        background.SetActive(false);
        mapNameText.gameObject.SetActive(false);

        hideCoroutine = null;
    }
}

using UnityEngine;

public class MiniMapUIController : MonoBehaviour
{
    [Header("Mini Map UI")]
    [SerializeField] private GameObject miniMapPanel = null;

    [Header("Mini Map Camera")]
    [SerializeField] private Camera miniMapCamera = null;

    private void Awake()
    {
        if (miniMapPanel != null)
        {
            miniMapPanel.SetActive(true);
        }

        if (miniMapCamera != null)
        {
            miniMapCamera.enabled = true;
        }
    }
}

using UnityEngine;
/// <summary>
/// 전체 지도 UI
/// </summary>
public class FullMapUIController : MonoBehaviour
{
    [Header("Full Map UI")]
    [SerializeField] private GameObject fullMapPanel = null;

    private void Awake()
    {
        if (fullMapPanel != null)
        {
            fullMapPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleFullMap();
        }
    }

    private void ToggleFullMap()
    {
        if (fullMapPanel == null)
        {
            return;
        }

        fullMapPanel.SetActive(!fullMapPanel.activeSelf);
    }
}

using UnityEngine;

public class PanelController : MonoBehaviour
{
    [Header("Settings Panels")]
    public GameObject graphicsPanel = null;
    public GameObject soundPanel = null;

    public void OpenGraphicsPanel()
    {
        if (graphicsPanel == null || soundPanel == null)
        {
            return;
        }

        graphicsPanel.SetActive(true);
        soundPanel.SetActive(false);
    }

    public void OpenSoundPanel()
    {
        if (graphicsPanel == null || soundPanel == null)
        {
            return;
        }

        graphicsPanel.SetActive(false);
        soundPanel.SetActive(true);
    }
}

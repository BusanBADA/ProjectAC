using UnityEngine;
using UnityEngine.UI;

public class OptionsController : MonoBehaviour
{
    [Header("Options Panel")]
    public GameObject optionsPanel = null;

    [Header("Volume Sliders")]
    public Slider masterVolumeSlider = null;
    public Slider bgmVolumeSlider = null;
    public Slider sfxVolumeSlider = null;

    [Header("Volume Texts")]
    public TMPro.TextMeshProUGUI masterVolumeText = null;
    public TMPro.TextMeshProUGUI bgmVolumeText = null;
    public TMPro.TextMeshProUGUI sfxVolumeText = null;

    [Header("Volume Settings")]
    public float masterVolume = 10f;
    public float bgmVolume = 10f;
    public float sfxVolume = 10f;

    public void OpenOptions()
    {
        if (optionsPanel == null)
        {
            return;
        }

        optionsPanel.SetActive(true);

        UpdateVolumeTexts();
    }

    public void UpdateVolumeTexts()
    {
        SetMasterVolume();
        SetBgmVolume();
        SetSfxVolume();
    }

    public void CloseOptions() 
    {
        if(optionsPanel == null) 
        {
            return; 
        }

        optionsPanel.SetActive(false);
    }

    public void SetMasterVolume()
    {
        if (masterVolumeSlider == null)
        {
            return;
        }

        masterVolume = masterVolumeSlider.value;

        if (masterVolumeText != null)
        {
            masterVolumeText.text = "MasterVolume : " + masterVolume.ToString("0");
        }
    }

    public void SetBgmVolume()
    {
        if (bgmVolumeSlider == null)
        {
            return;
        }

        bgmVolume = bgmVolumeSlider.value;

        if (bgmVolumeText != null)
        {
            bgmVolumeText.text = "BGMVolume : " + bgmVolume.ToString("0");
        }
    }

    public void SetSfxVolume()
    {
        if (sfxVolumeSlider == null)
        {
            return;
        }

        sfxVolume = sfxVolumeSlider.value;

        if (sfxVolumeText != null)
        {
            sfxVolumeText.text = "SfxVolume : " + sfxVolume.ToString("0");
        }
    }

    public float GetFinalBgmVolume()
    {
        return (masterVolume / 10f) * (bgmVolume / 10f);
    }

    public float GetFinalSfxVolume()
    {
        return (masterVolume / 10f) * (sfxVolume / 10f);
    }
}

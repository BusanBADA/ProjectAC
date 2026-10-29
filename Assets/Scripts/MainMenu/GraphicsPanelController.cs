using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using NUnit.Framework;

public class GraphicsPanelController : MonoBehaviour
{
    [Header("Resolution Settings")]
    public TMP_Dropdown resolutionDropdown = null;
    public Resolution[] resolutions = null;

    [Header("Display Settings")]
    public Toggle fullScreenToggle = null;

    private int currentResolutionIndex = 0;

    private void Start()
    {
        InitializeResolution();
        InitializeFullScreen();
    }

    public void InitializeResolution()
    {
        if (resolutionDropdown == null)
        {
            return;
        }

        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> resolutionOptions = new List<string>();

        currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string resolutionText = 
                resolutions[i].width + " x " + resolutions[i].height;

            resolutionOptions.Add(resolutionText);

            if (resolutions[i].width == Screen.width &&
                resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(resolutionOptions);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void ChangeResolution()
    {
        if (resolutionDropdown == null || resolutions == null)
        {
            return;
        }

        int selectedIndex = resolutionDropdown.value;

        if (selectedIndex < 0 || selectedIndex >= resolutions.Length)
        {
            return;
        }

        Resolution selectedResolution = resolutions[selectedIndex];

        Screen.SetResolution(
            selectedResolution.width,
            selectedResolution.height,
            Screen.fullScreen);
    }

    public void InitializeFullScreen()
    {
        if (fullScreenToggle == null)
        {
            return;
        }

        fullScreenToggle.isOn = Screen.fullScreen;
    }

    public void ChangeFullScreen()
    {
        if (fullScreenToggle == null)
        {
            return;
        }

        Screen.fullScreen = fullScreenToggle.isOn;
    }
}

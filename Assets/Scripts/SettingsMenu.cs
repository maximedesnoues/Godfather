using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    [SerializeField] private Slider soundSlider;

    [SerializeField] private Dropdown resolutionsDropdown;
    [SerializeField] private Dropdown qualitiesDropdown;

    private void Start()
    {
        InitializeSettings();
        ApplySettings();

        soundSlider.onValueChanged.AddListener(delegate { UpdateSettings(); });
        resolutionsDropdown.onValueChanged.AddListener(delegate { UpdateSettings(); });
        qualitiesDropdown.onValueChanged.AddListener(delegate { UpdateSettings(); });
    }

    private void InitializeSettings()
    {
        soundSlider.value = 0.5f;

        Resolution[] resolutions = Screen.resolutions;
        resolutionsDropdown.ClearOptions();
        List<string> resolutionOptions = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
#if UNITY_6000_0_OR_NEWER
            var rr = resolutions[i].refreshRateRatio;
            string option = $"{resolutions[i].width} x {resolutions[i].height} ({rr.numerator / rr.denominator} Hz)";
#else
    string option = $"{resolutions[i].width} x {resolutions[i].height} ({resolutions[i].refreshRate} Hz)";
#endif
            resolutionOptions.Add(option);

            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionsDropdown.AddOptions(resolutionOptions);
        resolutionsDropdown.value = currentResolutionIndex;
        resolutionsDropdown.RefreshShownValue();

        string[] qualities = QualitySettings.names;
        qualitiesDropdown.ClearOptions();
        qualitiesDropdown.AddOptions(new List<string>(qualities));

        int currentQuality = QualitySettings.GetQualityLevel();
        if (currentQuality <= 0)
        {
            currentQuality = qualities.Length - 1;
            QualitySettings.SetQualityLevel(currentQuality);
        }

        qualitiesDropdown.value = currentQuality;
        qualitiesDropdown.RefreshShownValue();
    }

    public void UpdateSettings()
    {
        ApplySettings();
    }

    private void ApplySettings()
    {
        SetVolume("SoundVolume", soundSlider.value);

        var resolutions = Screen.resolutions;
        if (resolutions.Length > 0)
        {
            int idx = Mathf.Clamp(resolutionsDropdown.value, 0, resolutions.Length - 1);
#if UNITY_6000_0_OR_NEWER
            Screen.SetResolution(resolutions[idx].width, resolutions[idx].height, Screen.fullScreenMode);
#else
        Screen.SetResolution(resolutions[idx].width, resolutions[idx].height, Screen.fullScreen);
#endif
        }

        int q = Mathf.Clamp(qualitiesDropdown.value, 0, QualitySettings.names.Length - 1);
        QualitySettings.SetQualityLevel(q, true);
    }

    private void SetVolume(string parameter, float value)
    {
        float dB;

        if (value <= 0f)
        {
            dB = -80f;
        }
        else
        {
            dB = Mathf.Log10(value) * 20f;
        }

        audioMixer.SetFloat(parameter, dB);
    }
}
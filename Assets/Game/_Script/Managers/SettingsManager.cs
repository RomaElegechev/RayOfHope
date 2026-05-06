using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("SettingPanels")]
    [SerializeField] private GameObject _mainPanel;
    [SerializeField] private GameObject _volumePanel;
    [SerializeField] private GameObject _screenPanel;
    [Header("Volume: Sliders")]
    [SerializeField] private Slider _masterSlider;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _soundSlider;
    [Header("Volume: Percents")]
    [SerializeField] private TMP_Text _masterPercent;
    [SerializeField] private TMP_Text _musicPercent;
    [SerializeField] private TMP_Text _soundPercent;
    [Header("Screen")]
    [SerializeField] private TMP_Dropdown _resolutionDropdown;
    [SerializeField] private TMP_Dropdown _fullscreenModeDropdown;

    private Resolution[] _availableResolutions;
    private List<Resolution> _filteredResolutions = new List<Resolution>();

    private void Awake()
    {
        _masterSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        _musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        _soundSlider.onValueChanged.AddListener(OnSoundVolumeChanged);
    }

    private void Start()
    {
        LoadSettings();
        SetupFullscreenDropdown();
        SetupResolutionDropdown();
    }

    public void OnVolumeButton()
    {
        SoundManager.Instance.PlaySFX("Tap");
        _mainPanel.SetActive(false);
        _volumePanel.SetActive(true);
    }
    public void OnScreenButton()
    {
        SoundManager.Instance.PlaySFX("Tap");
        _mainPanel.SetActive(false);
        _screenPanel.SetActive(true);
    }
    public void OnBackButton()
    {
        SoundManager.Instance.PlaySFX("Tap");
        _volumePanel.SetActive(false);
        _screenPanel.SetActive(false);
        _mainPanel.SetActive(true);
    }
    public void OnExitButton()
    {
        SoundManager.Instance.PlaySFX("Tap");
        gameObject.SetActive(false);
    }


    private void LoadSettings()
    {
        SoundManager.Instance.masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        SoundManager.Instance.musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        SoundManager.Instance.soundVolume = PlayerPrefs.GetFloat("SoundVolume", 1f);

        _masterSlider.value = SoundManager.Instance.masterVolume;
        _musicSlider.value = SoundManager.Instance.musicVolume;
        _soundSlider.value = SoundManager.Instance.soundVolume;

        int width = PlayerPrefs.GetInt("ResolutionWidth", Screen.currentResolution.width);
        int height = PlayerPrefs.GetInt("ResolutionHeight", Screen.currentResolution.height);
        Screen.SetResolution(width, height, Screen.fullScreenMode);

        int modeIndex = PlayerPrefs.GetInt("FullscreenMode");
        switch (modeIndex)
        {
            case 0:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
            case 1:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
            default:
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
        }

        UpdatePercentTexts();
    }


    private void OnMasterVolumeChanged(float value)
    {
        SoundManager.Instance.masterVolume = value;
        SoundManager.Instance.UpdateMusicVolume();
        UpdatePercentTexts();
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    private void OnMusicVolumeChanged(float value)
    {
        SoundManager.Instance.musicVolume = value;
        SoundManager.Instance.UpdateMusicVolume();
        UpdatePercentTexts();
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    private void OnSoundVolumeChanged(float value)
    {
        SoundManager.Instance.soundVolume = value;
        UpdatePercentTexts();
        PlayerPrefs.SetFloat("SoundVolume", value);
    }

    private void UpdatePercentTexts()
    {
        _masterPercent.text = Mathf.RoundToInt(_masterSlider.value * 100) + "%";
        _musicPercent.text = Mathf.RoundToInt(_musicSlider.value * 100) + "%";
        _soundPercent.text = Mathf.RoundToInt(_soundSlider.value * 100) + "%";
    }


    private void SetupResolutionDropdown()
    {
        _availableResolutions = Screen.resolutions;

        _filteredResolutions = _availableResolutions
            .GroupBy(res => new { res.width, res.height })
            .Select(group => group.First())
            .ToList();

        _resolutionDropdown.ClearOptions();

        List<string> resolutionOptions = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < _filteredResolutions.Count; i++)
        {
            string option = $"{_filteredResolutions[i].width} x {_filteredResolutions[i].height}";
            resolutionOptions.Add(option);

            if (_filteredResolutions[i].width == Screen.currentResolution.width
                && _filteredResolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        _resolutionDropdown.AddOptions(resolutionOptions);
        _resolutionDropdown.value = currentResolutionIndex;
        _resolutionDropdown.RefreshShownValue();

        _resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
    }

    private void OnResolutionChanged(int index)
    {
        SoundManager.Instance.PlaySFX("Tap");
        Resolution selecredResolution = _filteredResolutions[index];

        Screen.SetResolution(selecredResolution.width, selecredResolution.height, Screen.fullScreenMode);

        PlayerPrefs.SetInt("ResolutionWidth", selecredResolution.width);
        PlayerPrefs.SetInt("ResolutionHeight", selecredResolution.height);
    }


    private void SetupFullscreenDropdown()
    {
        _fullscreenModeDropdown.ClearOptions();

        List<string> modeOptions = new List<string>() { "Оконный", "Во весь экран (без рамок)", "Полноэкранный" };
        _fullscreenModeDropdown.AddOptions(modeOptions);

        switch (Screen.fullScreenMode)
        {
            case FullScreenMode.Windowed:
                _fullscreenModeDropdown.value = 0;
                break;
            case FullScreenMode.FullScreenWindow:
                _fullscreenModeDropdown.value = 1;
                break;
            case FullScreenMode.ExclusiveFullScreen:
                _fullscreenModeDropdown.value = 2;
                break;
        }
        _fullscreenModeDropdown.RefreshShownValue();

        _fullscreenModeDropdown.onValueChanged.AddListener(OnFullscreenModeChanged);
    }

    private void OnFullscreenModeChanged(int index)
    {
        SoundManager.Instance.PlaySFX("Tap");
        FullScreenMode newMode;
        switch (index)
        {
            case 0:
                newMode = FullScreenMode.Windowed;
                break;
            case 1:
                newMode = FullScreenMode.FullScreenWindow;
                break;
            default:
                newMode = FullScreenMode.ExclusiveFullScreen;
                break;
        }

        Screen.fullScreenMode = newMode;

        PlayerPrefs.SetInt("FullscreenMode", index);
    }
}

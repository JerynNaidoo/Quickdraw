using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [Header("Sound Settings")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Gameplay Settings")]
    [SerializeField] private Slider lookSensitivitySlider;

    private void Start()
    {
        if (PlayerPrefs.HasKey("masterVolume"))
        {
            LoadSettings();
        }
        else
        {
            SetMasterVolume();
            SetMusicVolume();
            SetSFXVolume();
            SetLookSensitivity();
        }
    }

    public void SetMasterVolume() {         
        float masterVolume = masterSlider.value;
        audioMixer.SetFloat("master", Mathf.Log10(masterVolume) * 20);
        PlayerPrefs.SetFloat("masterVolume", masterVolume);
    }

    public void SetMusicVolume()
    {
        float musicVolume = musicSlider.value;
        audioMixer.SetFloat("music", Mathf.Log10(musicVolume) * 20);
        PlayerPrefs.SetFloat("musicVolume", musicVolume);
    }

    public void SetSFXVolume()
    {
        float sfxVolume = sfxSlider.value;
        audioMixer.SetFloat("sfx", Mathf.Log10(sfxVolume) * 20);
        PlayerPrefs.SetFloat("sfxVolume", sfxVolume);
    }

    public void SetLookSensitivity()
    {
        float sensitivity = lookSensitivitySlider.value;
        PlayerPrefs.SetFloat("lookSensitivity", sensitivity);

        // Update player immediately if scene is active
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
            player.UpdateSensitivity(sensitivity);
    }

    private void LoadSettings()
    {
        masterSlider.value = PlayerPrefs.GetFloat("masterVolume");
        SetMasterVolume();
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        SetMusicVolume();
        sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume");
        SetSFXVolume();

        if (lookSensitivitySlider != null)
        {
            float sensitivity = PlayerPrefs.GetFloat("lookSensitivity", 0.5f);
            lookSensitivitySlider.value = sensitivity;
        }
    }
}

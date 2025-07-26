using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsController : MonoBehaviour
{
    [Header("Audio Settings")]
    public Slider musicVolumeSlider;
    public TextMeshProUGUI musicVolumeText;
    
    [Header("Controls Display")]
    public TextMeshProUGUI controlsText;
    
    private void Start()
    {
        // Mevcut müzik seviyesini yükle
        LoadMusicVolume();
        
        // Slider değişim eventi
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }
    }
    
    private void LoadMusicVolume()
    {
        // PlayerPrefs'ten müzik seviyesini yükle (varsayılan 0.7)
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
        
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = savedVolume;
        }
        
        // MusicManager'a ses seviyesini uygula
        ApplyMusicVolume(savedVolume);
        UpdateVolumeText(savedVolume);
    }
    
    public void OnMusicVolumeChanged(float value)
    {
        // Ses seviyesini kaydet
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();
        
        // MusicManager'a uygula
        ApplyMusicVolume(value);
        UpdateVolumeText(value);
    }
    
    private void ApplyMusicVolume(float volume)
    {
        // MusicManager varsa ses seviyesini güncelle
        MusicManager musicManager = FindObjectOfType<MusicManager>();
        if (musicManager != null)
        {
            musicManager.SetVolume(volume);
        }
        else
        {
            // Eğer MusicManager yoksa AudioListener volume'unu ayarla
            AudioListener.volume = volume;
        }
    }
    
    private void UpdateVolumeText(float value)
    {
        if (musicVolumeText != null)
        {
            musicVolumeText.text = Mathf.RoundToInt(value * 100) + "%";
        }
    }
    

} 
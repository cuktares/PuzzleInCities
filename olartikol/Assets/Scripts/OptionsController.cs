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
    
    [Header("Mute Toggle")]
    public Button muteToggleButton;
    public Sprite soundOnIcon;
    public Sprite soundOffIcon;
    
    private bool isMuted = false;
    private float volumeBeforeMute = 0.7f;
    
    private void Start()
    {
        // Mevcut müzik seviyesini yükle
        LoadMusicVolume();
        
        // Mute durumunu yükle
        LoadMuteState();
        
        // Slider değişim eventi
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }
        
        // Mute toggle butonu eventi
        if (muteToggleButton != null)
        {
            muteToggleButton.onClick.AddListener(ToggleMute);
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
        // Eğer slider ile ses açılırsa mute durumunu kaldır
        if (value > 0 && isMuted)
        {
            isMuted = false;
            PlayerPrefs.SetInt("IsMuted", 0);
            UpdateMuteButtonIcon();
        }
        // Eğer slider ile ses kapatılırsa mute durumunu aktif et
        else if (value == 0 && !isMuted)
        {
            isMuted = true;
            PlayerPrefs.SetInt("IsMuted", 1);
            UpdateMuteButtonIcon();
        }
        
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
    
    private void LoadMuteState()
    {
        // Mute durumunu PlayerPrefs'ten yükle
        isMuted = PlayerPrefs.GetInt("IsMuted", 0) == 1;
        volumeBeforeMute = PlayerPrefs.GetFloat("VolumeBeforeMute", 0.7f);
        
        // Eğer mute'lu başladıysak ses seviyesini 0 yap
        if (isMuted)
        {
            ApplyMusicVolume(0f);
            if (musicVolumeSlider != null)
            {
                musicVolumeSlider.value = 0f;
            }
        }
        
        // Button icon'unu güncelle
        UpdateMuteButtonIcon();
    }
    
    public void ToggleMute()
    {
        isMuted = !isMuted;
        
        if (isMuted)
        {
            // Mute yap - mevcut ses seviyesini kaydet ve sesi kapat
            if (musicVolumeSlider != null && musicVolumeSlider.value > 0)
            {
                volumeBeforeMute = musicVolumeSlider.value;
                PlayerPrefs.SetFloat("VolumeBeforeMute", volumeBeforeMute);
            }
            
            ApplyMusicVolume(0f);
            if (musicVolumeSlider != null)
            {
                musicVolumeSlider.value = 0f;
            }
        }
        else
        {
            // Mute'u kaldır - önceki ses seviyesini geri getir
            ApplyMusicVolume(volumeBeforeMute);
            if (musicVolumeSlider != null)
            {
                musicVolumeSlider.value = volumeBeforeMute;
            }
        }
        
        // Mute durumunu kaydet
        PlayerPrefs.SetInt("IsMuted", isMuted ? 1 : 0);
        PlayerPrefs.Save();
        
        // Button icon'unu güncelle
        UpdateMuteButtonIcon();
    }
    
    private void UpdateMuteButtonIcon()
    {
        if (muteToggleButton != null)
        {
            Image buttonImage = muteToggleButton.GetComponent<Image>();
            if (buttonImage != null)
            {
                if (isMuted && soundOffIcon != null)
                {
                    buttonImage.sprite = soundOffIcon;
                }
                else if (!isMuted && soundOnIcon != null)
                {
                    buttonImage.sprite = soundOnIcon;
                }
            }
        }
    }

} 
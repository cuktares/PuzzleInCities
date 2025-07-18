using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

namespace StarterAssets
{
    public class MainMenuController : MonoBehaviour
    {
        [Header("Menü Panelleri")]
        public GameObject mainMenuPanel;
        public GameObject settingsPanel;
        public GameObject levelSelectPanel;
        
        [Header("Ana Menü Butonları")]
        public Button playButton;
        public Button settingsButton;
        public Button quitButton;
        
        [Header("Ayarlar Menüsü")]
        public Slider musicVolumeSlider;
        public Slider effectsVolumeSlider;
        public Toggle musicToggle;
        public Toggle effectsToggle;
        public Button settingsBackButton;
        
        [Header("Level Seçim")]
        public Transform levelButtonContainer;
        public GameObject levelButtonPrefab;
        public Button levelSelectBackButton;
        
        [Header("Animasyon")]
        public CanvasGroup mainMenuCanvasGroup;
        public float fadeDuration = 0.5f;
        
        [Header("Ses")]
        public AudioClip buttonClickSound;
        public AudioClip menuMusic;
        
        private AudioSource audioSource;
        private int currentLevel = 1;
        private const int TOTAL_LEVELS = 10; // Toplam level sayısını buradan ayarlayın
        
        private void Start()
        {
            InitializeAudio();
            SetupButtons();
            LoadSettings();
            ShowMainMenu();
            
            // Menü müziğini başlat
            if (MusicManager.Instance != null && menuMusic != null)
            {
                MusicManager.Instance.musicSource.clip = menuMusic;
                MusicManager.Instance.musicSource.Play();
            }
        }
        
        private void InitializeAudio()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            audioSource.playOnAwake = false;
        }
        
        private void SetupButtons()
        {
            // Ana menü butonları
            if (playButton != null)
                playButton.onClick.AddListener(OnPlayButtonClicked);
                
            if (settingsButton != null)
                settingsButton.onClick.AddListener(OnSettingsButtonClicked);
                
            if (quitButton != null)
                quitButton.onClick.AddListener(OnQuitButtonClicked);
            
            // Ayarlar butonları
            if (settingsBackButton != null)
                settingsBackButton.onClick.AddListener(OnSettingsBackClicked);
                
            if (levelSelectBackButton != null)
                levelSelectBackButton.onClick.AddListener(OnLevelSelectBackClicked);
            
            // Slider ve toggle eventleri
            if (musicVolumeSlider != null)
                musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
                
            if (effectsVolumeSlider != null)
                effectsVolumeSlider.onValueChanged.AddListener(OnEffectsVolumeChanged);
                
            if (musicToggle != null)
                musicToggle.onValueChanged.AddListener(OnMusicToggleChanged);
                
            if (effectsToggle != null)
                effectsToggle.onValueChanged.AddListener(OnEffectsToggleChanged);
        }
        
        private void LoadSettings()
        {
            // Müzik ayarlarını yükle
            if (MusicManager.Instance != null)
            {
                float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
                bool musicMuted = PlayerPrefs.GetInt("MusicMuted", 0) == 1;
                
                if (musicVolumeSlider != null)
                    musicVolumeSlider.value = musicVolume;
                    
                if (musicToggle != null)
                    musicToggle.isOn = !musicMuted;
                    
                MusicManager.Instance.SetVolume(musicVolume);
                if (musicMuted)
                    MusicManager.Instance.ToggleMusic();
            }
            
            // Efekt ayarlarını yükle
            if (SoundManager.Instance != null)
            {
                float effectsVolume = PlayerPrefs.GetFloat("EffectsVolume", 0.5f);
                bool effectsMuted = PlayerPrefs.GetInt("EffectsMuted", 0) == 1;
                
                if (effectsVolumeSlider != null)
                    effectsVolumeSlider.value = effectsVolume;
                    
                if (effectsToggle != null)
                    effectsToggle.isOn = !effectsMuted;
                    
                SoundManager.Instance.SetEffectsVolume(effectsVolume);
                if (effectsMuted)
                    SoundManager.Instance.ToggleEffects();
            }
            
            // Mevcut level'i yükle
            currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        }
        
        private void ShowMainMenu()
        {
            mainMenuPanel.SetActive(true);
            settingsPanel.SetActive(false);
            levelSelectPanel.SetActive(false);
            
            if (mainMenuCanvasGroup != null)
                StartCoroutine(FadeIn(mainMenuCanvasGroup));
        }
        
        // Ana Menü Butonları
        private void OnPlayButtonClicked()
        {
            PlayButtonSound();
            ShowLevelSelect();
        }
        
        private void OnSettingsButtonClicked()
        {
            PlayButtonSound();
            ShowSettings();
        }
        
        private void OnQuitButtonClicked()
        {
            PlayButtonSound();
            
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
        
        // Ayarlar Menüsü
        private void ShowSettings()
        {
            mainMenuPanel.SetActive(false);
            settingsPanel.SetActive(true);
            levelSelectPanel.SetActive(false);
        }
        
        private void OnSettingsBackClicked()
        {
            PlayButtonSound();
            ShowMainMenu();
        }
        
        private void OnMusicVolumeChanged(float value)
        {
            if (MusicManager.Instance != null)
            {
                MusicManager.Instance.SetVolume(value);
            }
        }
        
        private void OnEffectsVolumeChanged(float value)
        {
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.SetEffectsVolume(value);
            }
        }
        
        private void OnMusicToggleChanged(bool isOn)
        {
            if (MusicManager.Instance != null)
            {
                if (!isOn && !MusicManager.Instance.IsMuted())
                    MusicManager.Instance.ToggleMusic();
                else if (isOn && MusicManager.Instance.IsMuted())
                    MusicManager.Instance.ToggleMusic();
            }
        }
        
        private void OnEffectsToggleChanged(bool isOn)
        {
            if (SoundManager.Instance != null)
            {
                if (!isOn && !SoundManager.Instance.IsEffectsMuted())
                    SoundManager.Instance.ToggleEffects();
                else if (isOn && SoundManager.Instance.IsEffectsMuted())
                    SoundManager.Instance.ToggleEffects();
            }
        }
        
        // Level Seçim
        private void ShowLevelSelect()
        {
            mainMenuPanel.SetActive(false);
            settingsPanel.SetActive(false);
            levelSelectPanel.SetActive(true);
            
            CreateLevelButtons();
        }
        
        private void CreateLevelButtons()
        {
            // Mevcut butonları temizle
            if (levelButtonContainer != null)
            {
                foreach (Transform child in levelButtonContainer)
                {
                    Destroy(child.gameObject);
                }
            }
            
            // Level butonlarını oluştur
            for (int i = 1; i <= TOTAL_LEVELS; i++)
            {
                if (levelButtonPrefab != null && levelButtonContainer != null)
                {
                    GameObject buttonObj = Instantiate(levelButtonPrefab, levelButtonContainer);
                    Button levelButton = buttonObj.GetComponent<Button>();
                    Text buttonText = buttonObj.GetComponentInChildren<Text>();
                    
                    if (buttonText != null)
                        buttonText.text = $"Level {i}";
                    
                    int levelIndex = i; // Closure için
                    if (levelButton != null)
                    {
                        levelButton.onClick.AddListener(() => OnLevelButtonClicked(levelIndex));
                        
                        // Eğer level henüz açılmamışsa butonu devre dışı bırak
                        if (i > currentLevel)
                        {
                            levelButton.interactable = false;
                            if (buttonText != null)
                                buttonText.color = Color.gray;
                        }
                    }
                }
            }
        }
        
        private void OnLevelButtonClicked(int levelIndex)
        {
            PlayButtonSound();
            
            // Level'i kaydet ve yükle
            PlayerPrefs.SetInt("CurrentLevel", levelIndex);
            PlayerPrefs.Save();
            
            // Level'i yükle (build index'e göre)
            int sceneIndex = levelIndex; // Level 1 = Scene 1, Level 2 = Scene 2, vb.
            if (sceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(sceneIndex);
            }
            else
            {
                Debug.LogWarning($"Level {levelIndex} henüz build'de yok!");
            }
        }
        
        private void OnLevelSelectBackClicked()
        {
            PlayButtonSound();
            ShowMainMenu();
        }
        
        // Yardımcı metodlar
        private void PlayButtonSound()
        {
            if (buttonClickSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(buttonClickSound);
            }
        }
        
        private IEnumerator FadeIn(CanvasGroup canvasGroup)
        {
            canvasGroup.alpha = 0f;
            
            while (canvasGroup.alpha < 1f)
            {
                canvasGroup.alpha += Time.deltaTime / fadeDuration;
                yield return null;
            }
            
            canvasGroup.alpha = 1f;
        }
    }
} 
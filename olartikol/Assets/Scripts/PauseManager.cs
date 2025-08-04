using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [Header("Pause UI Elements")]
    public GameObject pauseMenuPanel;
    public Button pauseButton;
    
    [Header("Pause Menu Buttons")]
    public Button resumeButton;
    public Button restartButton;
    public Button mainMenuButton;
    public Button settingsButton;
    
    [Header("Settings Panel")]
    public GameObject settingsPanel;
    public Button settingsBackButton;
    
    private bool isPaused = false;
    private bool wasAudioPaused = false;
    private bool isLevelCompleted = false;

    private void Start()
    {
        // Başlangıçta pause menüsü kapalı
        pauseMenuPanel.SetActive(false);
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
        
        // Button event'lerini bağla
        if (pauseButton != null)
            pauseButton.onClick.AddListener(PauseGame);
            
        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);
            
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartLevel);
            
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(GoToMainMenu);
            
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OpenSettings);
            
        if (settingsBackButton != null)
            settingsBackButton.onClick.AddListener(CloseSettings);
    }
    
    private void Update()
    {
        // ESC tuşu ile pause (PC test için)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingsPanel != null && settingsPanel.activeInHierarchy)
            {
                CloseSettings();
            }
            else if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
    
    public void PauseGame()
    {
        // Level tamamlandıysa pause yapma
        if (isLevelCompleted) return;
        
        isPaused = true;
        Time.timeScale = 0f;
        pauseMenuPanel.SetActive(true);
        
        // Müziği duraklat
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PauseMusic();
            wasAudioPaused = true;
        }
        
        // Cursor'u mobilde göster (gerekirse)
        #if !UNITY_ANDROID && !UNITY_IOS
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        #endif
    }
    
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pauseMenuPanel.SetActive(false);
        
        // Settings paneli de kapalı olduğundan emin ol
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
        
        // Müziği devam ettir
        if (MusicManager.Instance != null && wasAudioPaused)
        {
            MusicManager.Instance.ResumeMusic();
            wasAudioPaused = false;
        }
        
        // Cursor'u tekrar kilitle (gerekirse)
        #if !UNITY_ANDROID && !UNITY_IOS
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        #endif
    }
    
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        
        // Level completion durumunu sıfırla
        SetLevelCompleted(false);
        
        // Klon sistemini sıfırla
        var playerController = FindObjectOfType<StarterAssets.EnhancedThirdPersonController>();
        if (playerController != null)
        {
            playerController.ResetCloneSystem();
        }
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0); // MainMenu sahnesinin indeksi
    }
    
    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            pauseMenuPanel.SetActive(false);
            settingsPanel.SetActive(true);
        }
    }
    
    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
            pauseMenuPanel.SetActive(true);
        }
    }
    
    // Level tamamlandığında çağırılacak metod
    public void SetLevelCompleted(bool completed)
    {
        isLevelCompleted = completed;
        
        // Level tamamlandıysa pause butonunu devre dışı bırak
        if (pauseButton != null)
        {
            pauseButton.interactable = !completed;
        }
        
        // Eğer level tamamlandıysa ve pause menüsü açıksa kapat
        if (completed && isPaused)
        {
            ResumeGame();
        }
    }
    
    // Diğer scriptler için pause durumunu kontrol etme
    public bool IsPaused()
    {
        return isPaused;
    }
    
    // Level completion durumunu kontrol etme
    public bool IsLevelCompleted()
    {
        return isLevelCompleted;
    }
} 
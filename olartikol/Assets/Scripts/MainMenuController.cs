using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Menu Panels")]
    public GameObject mainMenuPanel;
    public GameObject optionsPanel;
    
    [Header("Main Menu Buttons")]
    public Button startButton;
    public Button levelSelectButton;
    public Button optionsButton;
    public Button exitButton;
    
    [Header("Options Buttons")]
    public Button backButton;
    
    private void Start()
    {
        // Ana menüyü göster, options'ı gizle
        ShowMainMenu();
        
        // Button click eventleri
        if (startButton != null)
            startButton.onClick.AddListener(StartGame);
            
        if (levelSelectButton != null)
        {
            levelSelectButton.onClick.AddListener(ShowLevelSelection);
            Debug.Log("Level Select button bağlandı.");
        }
        else
        {
            Debug.LogError("Level Select Button Inspector'da atanmamış!");
        }
            
        if (optionsButton != null)
            optionsButton.onClick.AddListener(ShowOptions);
            
        if (exitButton != null)
            exitButton.onClick.AddListener(ExitGame);
            
        if (backButton != null)
            backButton.onClick.AddListener(ShowMainMenu);
    }
    
    public void StartGame()
    {
        // İlk level'ı başlat
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.LoadLevel(1);
        }
        else
        {
            // Fallback: Build Settings'te 1. indeks
            SceneManager.LoadScene(1);
        }
    }
    
    public void ShowLevelSelection()
    {
        Debug.Log("ShowLevelSelection fonksiyonu çağrıldı!");
        
        LevelSelectionUI levelSelection = FindObjectOfType<LevelSelectionUI>();
        if (levelSelection != null)
        {
            Debug.Log("LevelSelectionUI bulundu, panel gösteriliyor.");
            
            // Ana menüyü gizle
            mainMenuPanel.SetActive(false);
            optionsPanel.SetActive(false);
            
            // Level seçim panelini göster
            levelSelection.ShowLevelSelection();
        }
        else
        {
            Debug.LogError("LevelSelectionUI bulunamadı! MainMenu sahnesine LevelSelectionUI scripti ve paneli eklediğinizden emin olun.");
        }
    }
    
    public void ShowOptions()
    {
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }
    
    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        optionsPanel.SetActive(false);
    }
    
    public void ExitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
} 
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelectionUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject levelSelectionPanel;
    public Transform levelButtonsParent;
    public Button backToMainMenuButton;
    
    [Header("Level Button Prefab")]
    public GameObject levelButtonPrefab;
    
    [Header("Colors")]
    public Color unlockedColor = Color.white;
    public Color lockedColor = Color.gray;
    public Color completedColor = Color.green;
    
    private LevelButton[] levelButtons;

    private void Start()
    {
        // Ana menüye dönüş butonu
        if (backToMainMenuButton != null)
            backToMainMenuButton.onClick.AddListener(BackToMainMenu);
            
        // Level butonlarını oluştur
        CreateLevelButtons();
        
        // Panel'i başlangıçta gizle
        if (levelSelectionPanel != null)
            levelSelectionPanel.SetActive(false);
    }
    
    private void CreateLevelButtons()
    {
        if (levelButtonPrefab == null || levelButtonsParent == null)
        {
            Debug.LogError("Level button prefab veya parent eksik!");
            return;
        }
        
        int totalLevels = LevelManager.Instance != null ? LevelManager.Instance.totalLevels : 20;
        levelButtons = new LevelButton[totalLevels];
        
        for (int i = 0; i < totalLevels; i++)
        {
            GameObject buttonObj = Instantiate(levelButtonPrefab, levelButtonsParent);
            LevelButton levelButton = buttonObj.GetComponent<LevelButton>();
            
            if (levelButton != null)
            {
                levelButton.Initialize(i + 1); // Level numaraları 1'den başlar
                levelButtons[i] = levelButton;
            }
        }
        
        // Buton durumlarını güncelle
        UpdateLevelButtons();
    }
    
    public void ShowLevelSelection()
    {
        if (levelSelectionPanel != null)
        {
            levelSelectionPanel.SetActive(true);
            UpdateLevelButtons();
        }
    }
    
    public void HideLevelSelection()
    {
        if (levelSelectionPanel != null)
            levelSelectionPanel.SetActive(false);
    }
    
    public void UpdateLevelButtons()
    {
        if (levelButtons == null || LevelManager.Instance == null) return;
        
        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (levelButtons[i] != null)
            {
                levelButtons[i].UpdateButtonState();
            }
        }
    }
    
    public void BackToMainMenu()
    {
        HideLevelSelection();
        
        // MainMenuController'ı bul ve ana menüyü göster
        MainMenuController mainMenuController = FindObjectOfType<MainMenuController>();
        if (mainMenuController != null)
        {
            mainMenuController.ShowMainMenu();
        }
    }
    
    private void OnEnable()
    {
        // Panel aktif olduğunda buton durumlarını güncelle
        UpdateLevelButtons();
    }
} 
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelButton : MonoBehaviour
{
    [Header("UI References")]
    public Button button;
    public TextMeshProUGUI levelNumberText;
    public GameObject lockIcon;
    public GameObject starIcon;
    
    [Header("Visual States")]
    public Color unlockedColor = Color.white;
    public Color lockedColor = Color.gray;
    public Color completedColor = Color.green;
    
    private int levelNumber;
    private Image buttonImage;

    private void Awake()
    {
        // Component referanslarını al
        if (button == null)
            button = GetComponent<Button>();
            
        if (buttonImage == null)
            buttonImage = GetComponent<Image>();
            
        // Button click eventi
        if (button != null)
            button.onClick.AddListener(OnLevelButtonClicked);
    }
    
    public void Initialize(int level)
    {
        levelNumber = level;
        
        // Level numarasını göster
        if (levelNumberText != null)
            levelNumberText.text = level.ToString();
            
        // Başlangıç durumunu ayarla
        UpdateButtonState();
    }
    
    public void UpdateButtonState()
    {
        if (LevelManager.Instance == null) return;
        
        bool isUnlocked = LevelManager.Instance.IsLevelUnlocked(levelNumber);
        bool isCompleted = LevelManager.Instance.IsLevelCompleted(levelNumber);
        
        // Button interactable durumu
        if (button != null)
            button.interactable = isUnlocked;
        
        // Görsel durumları
        if (isCompleted)
        {
            SetCompletedState();
        }
        else if (isUnlocked)
        {
            SetUnlockedState();
        }
        else
        {
            SetLockedState();
        }
    }
    
    private void SetUnlockedState()
    {
        // Buton rengi
        if (buttonImage != null)
            buttonImage.color = unlockedColor;
            
        // Kilit ikonu gizle
        if (lockIcon != null)
            lockIcon.SetActive(false);
            
        // Yıldız ikonu gizle
        if (starIcon != null)
            starIcon.SetActive(false);
            
        // Level numarası görünür
        if (levelNumberText != null)
        {
            levelNumberText.color = Color.black;
            levelNumberText.gameObject.SetActive(true);
        }
    }
    
    private void SetLockedState()
    {
        // Buton rengi
        if (buttonImage != null)
            buttonImage.color = lockedColor;
            
        // Kilit ikonu göster
        if (lockIcon != null)
            lockIcon.SetActive(true);
            
        // Yıldız ikonu gizle
        if (starIcon != null)
            starIcon.SetActive(false);
            
        // Level numarası gizle veya gri yap
        if (levelNumberText != null)
        {
            levelNumberText.color = Color.gray;
            levelNumberText.gameObject.SetActive(false);
        }
    }
    
    private void SetCompletedState()
    {
        // Buton rengi
        if (buttonImage != null)
            buttonImage.color = completedColor;
            
        // Kilit ikonu gizle
        if (lockIcon != null)
            lockIcon.SetActive(false);
            
        // Yıldız ikonu göster
        if (starIcon != null)
            starIcon.SetActive(true);
            
        // Level numarası görünür
        if (levelNumberText != null)
        {
            levelNumberText.color = Color.white;
            levelNumberText.gameObject.SetActive(true);
        }
    }
    
    private void OnLevelButtonClicked()
    {
        if (LevelManager.Instance == null) return;
        
        // Level'ın unlock olup olmadığını kontrol et
        if (LevelManager.Instance.IsLevelUnlocked(levelNumber))
        {
            // Level'ı yükle
            LevelManager.Instance.LoadLevel(levelNumber);
        }
        else
        {
            Debug.Log($"Level {levelNumber} is locked!");
        }
    }
} 
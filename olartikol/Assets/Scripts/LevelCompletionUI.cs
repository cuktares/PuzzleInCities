using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LevelCompletionUI : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject completionPanel;
    public CanvasGroup completionCanvasGroup;
    
    [Header("UI Elements")]
    public TextMeshProUGUI completionTitle;
    
    [Header("Buttons")]
    public Button nextLevelButton;
    public Button restartButton;
    public Button levelSelectButton;
    public Button mainMenuButton;
    
    [Header("Animation Settings")]
    public float fadeInDuration = 0.5f;
    public float scaleAnimationDuration = 0.3f;
    public AnimationCurve animationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    
    private string nextLevelName = "";
    private int nextSceneIndex = -1;
    private bool isAnimating = false;

    private void Start()
    {
        // Panel'i başlangıçta gizle
        if (completionPanel != null)
            completionPanel.SetActive(false);
            
        // Button event'lerini bağla
        if (nextLevelButton != null)
            nextLevelButton.onClick.AddListener(LoadNextLevel);
            
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartCurrentLevel);
            
        if (levelSelectButton != null)
            levelSelectButton.onClick.AddListener(GoToLevelSelection);
            
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(GoToMainMenu);
    }
    
    public void ShowCompletionUI(string nextLevel = "", int nextIndex = -1)
    {
        if (isAnimating) return;
        
        nextLevelName = nextLevel;
        nextSceneIndex = nextIndex;
        
        // Panel'i aktif et
        completionPanel.SetActive(true);
        
        // Mevcut level'ı tamamlandı olarak işaretle
        if (LevelManager.Instance != null)
        {
            int currentLevel = LevelManager.Instance.GetCurrentLevelNumber();
            if (currentLevel > 0)
            {
                LevelManager.Instance.CompleteLevel(currentLevel);
            }
        }
        
        // Next Level butonunu kontrol et
        if (nextLevelButton != null)
        {
            bool hasNextLevel = !string.IsNullOrEmpty(nextLevelName) || nextSceneIndex != -1;
            nextLevelButton.gameObject.SetActive(hasNextLevel);
        }
        
        // Animasyonu başlat
        StartCompletionAnimation();
    }
    
    private void StartCompletionAnimation()
    {
        isAnimating = true;
        
        // Başlangıç durumu
        if (completionCanvasGroup != null)
        {
            completionCanvasGroup.alpha = 0f;
            completionCanvasGroup.transform.localScale = Vector3.zero;
        }
        
        // Oyunu duraklat
        Time.timeScale = 0f;
        
        // Animasyonu başlat
        StartCoroutine(AnimateCompletion());
    }
    
    private IEnumerator AnimateCompletion()
    {
        if (completionCanvasGroup == null)
        {
            isAnimating = false;
            yield break;
        }
        
        float elapsed = 0f;
        
        // Fade in ve scale animasyonu
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = elapsed / fadeInDuration;
            
            // Eased progress hesapla
            float easedProgress = animationCurve.Evaluate(progress);
            
            // Alpha ve scale uygula
            completionCanvasGroup.alpha = easedProgress;
            completionCanvasGroup.transform.localScale = Vector3.one * easedProgress;
            
            yield return null;
        }
        
        // Final değerleri ayarla
        completionCanvasGroup.alpha = 1f;
        completionCanvasGroup.transform.localScale = Vector3.one;
        
        isAnimating = false;
    }
    
    public void LoadNextLevel()
    {
        if (isAnimating) return;
        
        Time.timeScale = 1f;
        
        if (!string.IsNullOrEmpty(nextLevelName))
        {
            SceneManager.LoadScene(nextLevelName);
        }
        else if (nextSceneIndex != -1 && nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            // Son level ise ana menüye dön
            GoToMainMenu();
        }
    }
    
    public void RestartCurrentLevel()
    {
        if (isAnimating) return;
        
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void GoToLevelSelection()
    {
        if (isAnimating) return;
        
        Time.timeScale = 1f;
        SceneManager.LoadScene(0); // MainMenu sahnesine dön ve level seçimini aç
    }
    
    public void GoToMainMenu()
    {
        if (isAnimating) return;
        
        Time.timeScale = 1f;
        SceneManager.LoadScene(0); // MainMenu sahnesinin indeksi
    }
    
    private void OnDestroy()
    {
        // Sahne değişirken time scale'i normale çevir
        Time.timeScale = 1f;
    }
} 
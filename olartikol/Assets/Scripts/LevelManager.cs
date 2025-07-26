using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    
    [Header("Level Settings")]
    public int totalLevels = 20;
    public string levelScenePrefix = "sahne"; // sahne1, sahne2, etc.
    
    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        // İlk level'ı unlock et (eğer daha önce kaydedilmemişse)
        if (!HasUnlockedLevel(1))
        {
            UnlockLevel(1);
        }
    }
    
    // Belirli bir level'ın unlock olup olmadığını kontrol et
    public bool IsLevelUnlocked(int levelNumber)
    {
        if (levelNumber <= 0 || levelNumber > totalLevels) return false;
        return PlayerPrefs.GetInt($"Level_{levelNumber}_Unlocked", levelNumber == 1 ? 1 : 0) == 1;
    }
    
    // Level'ı unlock et
    public void UnlockLevel(int levelNumber)
    {
        if (levelNumber <= 0 || levelNumber > totalLevels) return;
        
        PlayerPrefs.SetInt($"Level_{levelNumber}_Unlocked", 1);
        PlayerPrefs.Save();
        
        Debug.Log($"Level {levelNumber} unlocked!");
    }
    
    // Level'ı tamamlandı olarak işaretle ve sonraki level'ı unlock et
    public void CompleteLevel(int levelNumber)
    {
        if (levelNumber <= 0 || levelNumber > totalLevels) return;
        
        // Mevcut level'ı tamamlandı olarak işaretle
        PlayerPrefs.SetInt($"Level_{levelNumber}_Completed", 1);
        
        // Sonraki level'ı unlock et
        int nextLevel = levelNumber + 1;
        if (nextLevel <= totalLevels)
        {
            UnlockLevel(nextLevel);
        }
        
        PlayerPrefs.Save();
        Debug.Log($"Level {levelNumber} completed! Next level unlocked: {nextLevel}");
    }
    
    // Level'ın tamamlanıp tamamlanmadığını kontrol et
    public bool IsLevelCompleted(int levelNumber)
    {
        if (levelNumber <= 0 || levelNumber > totalLevels) return false;
        return PlayerPrefs.GetInt($"Level_{levelNumber}_Completed", 0) == 1;
    }
    
    // Level'ı yükle
    public void LoadLevel(int levelNumber)
    {
        if (levelNumber <= 0 || levelNumber > totalLevels) return;
        
        if (!IsLevelUnlocked(levelNumber))
        {
            Debug.LogWarning($"Level {levelNumber} is locked!");
            return;
        }
        
        string sceneName = levelScenePrefix + levelNumber.ToString();
        SceneManager.LoadScene(sceneName);
    }
    
    // Mevcut level'ı al (scene adından)
    public int GetCurrentLevelNumber()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        
        // levelScenePrefix'i kaldır ve sayıyı al
        if (currentSceneName.StartsWith(levelScenePrefix))
        {
            string numberPart = currentSceneName.Substring(levelScenePrefix.Length);
            if (int.TryParse(numberPart, out int levelNumber))
            {
                return levelNumber;
            }
        }
        
        return -1; // Level bulunamadı
    }
    
    // Sonraki level'ı al
    public int GetNextLevelNumber()
    {
        int currentLevel = GetCurrentLevelNumber();
        return currentLevel + 1;
    }
    
    // Sonraki level var mı?
    public bool HasNextLevel()
    {
        int nextLevel = GetNextLevelNumber();
        return nextLevel > 0 && nextLevel <= totalLevels;
    }
    
    // Daha önce unlock edilmiş bir level var mı kontrol et
    private bool HasUnlockedLevel(int levelNumber)
    {
        return PlayerPrefs.HasKey($"Level_{levelNumber}_Unlocked");
    }
    
    // Progress'i sıfırla (test için)
    [ContextMenu("Reset All Progress")]
    public void ResetAllProgress()
    {
        for (int i = 1; i <= totalLevels; i++)
        {
            PlayerPrefs.DeleteKey($"Level_{i}_Unlocked");
            PlayerPrefs.DeleteKey($"Level_{i}_Completed");
        }
        PlayerPrefs.Save();
        
        // İlk level'ı tekrar unlock et
        UnlockLevel(1);
        
        Debug.Log("All progress reset!");
    }
} 
using UnityEngine;
using UnityEngine.UI;

namespace StarterAssets
{
    public class LevelButton : MonoBehaviour
    {
        [Header("UI Bileşenleri")]
        public Text levelText;
        public Image backgroundImage;
        public Image lockIcon;
        
        [Header("Renkler")]
        public Color unlockedColor = Color.white;
        public Color lockedColor = Color.gray;
        public Color selectedColor = Color.yellow;
        
        private Button button;
        private bool isUnlocked = true;
        private bool isSelected = false;
        
        private void Awake()
        {
            button = GetComponent<Button>();
        }
        
        public void SetLevel(int levelNumber, bool unlocked)
        {
            isUnlocked = unlocked;
            
            if (levelText != null)
                levelText.text = $"Level {levelNumber}";
            
            if (button != null)
                button.interactable = unlocked;
            
            if (lockIcon != null)
                lockIcon.gameObject.SetActive(!unlocked);
            
            UpdateVisuals();
        }
        
        public void SetSelected(bool selected)
        {
            isSelected = selected;
            UpdateVisuals();
        }
        
        private void UpdateVisuals()
        {
            if (backgroundImage != null)
            {
                if (isSelected)
                    backgroundImage.color = selectedColor;
                else if (isUnlocked)
                    backgroundImage.color = unlockedColor;
                else
                    backgroundImage.color = lockedColor;
            }
            
            if (levelText != null)
            {
                if (isUnlocked)
                    levelText.color = Color.black;
                else
                    levelText.color = Color.gray;
            }
        }
    }
} 
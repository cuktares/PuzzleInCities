using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneRestartTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Karakter trigger'a değdi, sahne yeniden yükleniyor...");
            
            // Klon sistemini sıfırla
            var playerController = other.GetComponent<StarterAssets.EnhancedThirdPersonController>();
            if (playerController != null)
            {
                playerController.ResetCloneSystem();
            }
            
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
} 
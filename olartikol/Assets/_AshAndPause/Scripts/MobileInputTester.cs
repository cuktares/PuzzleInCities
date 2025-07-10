using UnityEngine;
using StarterAssets;

public class MobileInputTester : MonoBehaviour
{
    [Header("Test Settings")]
    public bool showDebugInfo = true;
    public KeyCode testKey = KeyCode.T;
    
    [Header("References")]
    public StarterAssetsInputs starterAssetsInputs;
    public InputSystemWrapper inputWrapper;
    
    private void Start()
    {
        // Otomatik referansları bul
        if (starterAssetsInputs == null)
            starterAssetsInputs = FindObjectOfType<StarterAssetsInputs>();
            
        if (inputWrapper == null)
            inputWrapper = InputSystemWrapper.Instance;
            
        if (starterAssetsInputs == null)
        {
            Debug.LogError("⚠️ StarterAssetsInputs bulunamadı! Mobil UI çalışmayacak.");
        }
        
        if (inputWrapper == null)
        {
            Debug.LogError("⚠️ InputSystemWrapper bulunamadı! Mobil input çalışmayacak.");
        }
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(testKey))
        {
            TestMobileInput();
        }
        
        if (showDebugInfo)
        {
            ShowDebugInfo();
        }
    }
    
    private void TestMobileInput()
    {
        Debug.Log("🧪 Mobil Input Test Başladı!");
        
        if (starterAssetsInputs != null)
        {
            Debug.Log($"📱 StarterAssetsInputs - Move: {starterAssetsInputs.move}, Jump: {starterAssetsInputs.jump}, Interact: {starterAssetsInputs.interact}");
        }
        
        if (inputWrapper != null)
        {
            Debug.Log($"🎮 InputWrapper - Movement: {inputWrapper.GetMovementInput()}, Jump: {inputWrapper.GetJumpPressed()}, Interact: {inputWrapper.GetInteractPressed()}");
        }
    }
    
    private void ShowDebugInfo()
    {
        if (starterAssetsInputs != null && inputWrapper != null)
        {
            // Sadece input var ise göster
            if (starterAssetsInputs.move != Vector2.zero || inputWrapper.GetMovementInput() != Vector3.zero)
            {
                Debug.Log($"📊 Movement - Mobile: {starterAssetsInputs.move} | Final: {inputWrapper.GetMovementInput()}");
            }
        }
    }
    
    private void OnGUI()
    {
        if (!showDebugInfo) return;
        
        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.Label("🧪 Mobil Input Test");
        GUILayout.Label($"Test Key: {testKey}");
        
        if (starterAssetsInputs != null)
        {
            GUILayout.Label($"📱 Mobile Move: {starterAssetsInputs.move}");
            GUILayout.Label($"📱 Mobile Jump: {starterAssetsInputs.jump}");
            GUILayout.Label($"📱 Mobile Interact: {starterAssetsInputs.interact}");
        }
        
        if (inputWrapper != null)
        {
            GUILayout.Label($"🎮 Final Move: {inputWrapper.GetMovementInput()}");
            GUILayout.Label($"🎮 Final Jump: {inputWrapper.GetJumpPressed()}");
            GUILayout.Label($"🎮 Final Interact: {inputWrapper.GetInteractPressed()}");
        }
        
        GUILayout.EndArea();
    }
} 
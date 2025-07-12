using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class JoystickPlayerExample : MonoBehaviour
{
    [Header("Joystick Settings")]
    public VariableJoystick variableJoystick;
    
    [Header("Mobile Integration")]
    public bool enableMobileInput = true;
    
    private StarterAssetsInputs starterAssetsInputs;
    private Rigidbody rb;

    void Start()
    {
        // StarterAssetsInputs'i bul
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        if (starterAssetsInputs == null)
        {
            Debug.LogError("StarterAssetsInputs bulunamadı! Lütfen Player'a StarterAssetsInputs ekleyin.");
            return;
        }

        // Rigidbody'yi bul (kullanmayacağız ama backup için)
        rb = GetComponent<Rigidbody>();
        
        // Mobile UI'yi aktif et
        if (enableMobileInput)
        {
            starterAssetsInputs.EnableMobileUI(true);
        }
        
        Debug.Log("✅ JoystickPlayerExample başlatıldı - StarterAssetsInputs entegrasyonu aktif!");
    }

    void Update()
    {
        if (starterAssetsInputs != null && variableJoystick != null && enableMobileInput)
        {
            // Joystick input'unu StarterAssetsInputs'e gönder
            Vector2 joystickInput = new Vector2(variableJoystick.Horizontal, variableJoystick.Vertical);
            starterAssetsInputs.VirtualMoveInput(joystickInput);
        }
    }

    // Eski FixedUpdate metodunu kaldırdık - artık EnhancedThirdPersonController hareket yönetiyor
}
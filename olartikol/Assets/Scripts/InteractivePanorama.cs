using UnityEngine;
using System.IO;

public class InteractivePanorama : MonoBehaviour
{
    [Header("Panorama Settings")]
    public Material panoramaMaterial;
    public string panoramaFileName = "MenuPanorama";
    public bool loadOnStart = true;
    
    [Header("Interaction Settings")]
    public float mouseSensitivity = 2f;
    public float touchSensitivity = 1f;
    public bool invertY = false;
    public float rotationSpeed = 10f;
    
    [Header("Auto Rotation")]
    public bool autoRotate = true;
    public float autoRotateSpeed = 5f;
    
    private Camera panoramaCamera;
    private Vector2 rotation = Vector2.zero;
    private bool isDragging = false;
    private Vector2 lastInputPosition;
    
    // Mobil input
    private Touch[] touches;

    private void Start()
    {
        // Panorama kamerasını bul veya oluştur
        panoramaCamera = GetComponent<Camera>();
        if (panoramaCamera == null)
        {
            panoramaCamera = Camera.main;
        }
        
        if (loadOnStart)
        {
            LoadPanoramaTexture();
        }
        
        // Başlangıç rotasyonu
        rotation.x = transform.eulerAngles.y;
        rotation.y = transform.eulerAngles.x;
    }
    
    private void Update()
    {
        HandleInput();
        
        // Auto rotation
        if (autoRotate && !isDragging)
        {
            rotation.x += autoRotateSpeed * Time.deltaTime;
        }
        
        // Rotasyonu uygula
        ApplyRotation();
    }
    
    private void HandleInput()
    {
        // Mobil touch input
        #if UNITY_ANDROID || UNITY_IOS
        HandleTouchInput();
        #else
        HandleMouseInput();
        #endif
    }
    
    private void HandleTouchInput()
    {
        touches = Input.touches;
        
        if (touches.Length == 1)
        {
            Touch touch = touches[0];
            
            if (touch.phase == TouchPhase.Began)
            {
                isDragging = true;
                lastInputPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved && isDragging)
            {
                Vector2 delta = touch.position - lastInputPosition;
                
                rotation.x -= delta.x * touchSensitivity * Time.deltaTime;
                rotation.y += delta.y * touchSensitivity * (invertY ? -1 : 1) * Time.deltaTime;
                
                // Y rotasyonunu sınırla
                rotation.y = Mathf.Clamp(rotation.y, -90f, 90f);
                
                lastInputPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isDragging = false;
            }
        }
        else
        {
            isDragging = false;
        }
    }
    
    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            lastInputPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            Vector2 delta = (Vector2)Input.mousePosition - lastInputPosition;
            
            rotation.x -= delta.x * mouseSensitivity * Time.deltaTime;
            rotation.y += delta.y * mouseSensitivity * (invertY ? -1 : 1) * Time.deltaTime;
            
            // Y rotasyonunu sınırla
            rotation.y = Mathf.Clamp(rotation.y, -90f, 90f);
            
            lastInputPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }
    
    private void ApplyRotation()
    {
        Quaternion targetRotation = Quaternion.Euler(rotation.y, rotation.x, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
    
    [ContextMenu("Load Panorama")]
    public void LoadPanoramaTexture()
    {
        // Önce Resources klasöründen yükle
        Texture2D panoramaTexture = Resources.Load<Texture2D>(panoramaFileName);
        
        if (panoramaTexture == null)
        {
            // Resources'ta yoksa persistent data'dan yükle
            LoadFromPersistentData();
            return;
        }
        
        ApplyPanoramaTexture(panoramaTexture);
    }
    
    private void LoadFromPersistentData()
    {
        string[] extensions = { ".jpg", ".png" };
        
        foreach (string ext in extensions)
        {
            string filePath = Path.Combine(Application.persistentDataPath, panoramaFileName + ext);
            
            if (File.Exists(filePath))
            {
                byte[] data = File.ReadAllBytes(filePath);
                Texture2D texture = new Texture2D(2, 2);
                
                if (texture.LoadImage(data))
                {
                    ApplyPanoramaTexture(texture);
                    Debug.Log($"Panorama yüklendi: {filePath}");
                    return;
                }
            }
        }
        
        Debug.LogWarning($"Panorama dosyası bulunamadı: {panoramaFileName}");
    }
    
    private void ApplyPanoramaTexture(Texture2D texture)
    {
        if (panoramaMaterial != null)
        {
            panoramaMaterial.mainTexture = texture;
            Debug.Log("Panorama texture material'a uygulandı");
        }
        else
        {
            Debug.LogWarning("Panorama material atanmamış!");
        }
    }
    
    // Dış scriptler için kontrol fonksiyonları
    public void EnableAutoRotation()
    {
        autoRotate = true;
    }
    
    public void DisableAutoRotation()
    {
        autoRotate = false;
    }
    
    public void SetRotation(float x, float y)
    {
        rotation.x = x;
        rotation.y = Mathf.Clamp(y, -90f, 90f);
    }
    
    public void ResetRotation()
    {
        rotation = Vector2.zero;
    }
} 
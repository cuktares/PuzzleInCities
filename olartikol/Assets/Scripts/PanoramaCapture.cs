using UnityEngine;
using System.IO;

public class PanoramaCapture : MonoBehaviour
{
    [Header("Capture Settings")]
    public Camera captureCamera;
    public int panoramaWidth = 2048;
    public bool captureAsJPEG = true;
    
    [Header("Auto Capture")]
    public bool autoCaptureOnStart = false;
    public float captureDelay = 2f;
    
    [Header("Save Settings")]
    public string fileName = "MenuPanorama";
    
    private void Start()
    {
        if (autoCaptureOnStart)
        {
            Invoke(nameof(CapturePanorama), captureDelay);
        }
    }
    
    [ContextMenu("Capture Panorama")]
    public void CapturePanorama()
    {
        if (captureCamera == null)
        {
            captureCamera = Camera.main;
            if (captureCamera == null)
            {
                Debug.LogError("Kamera bulunamadı!");
                return;
            }
        }
        
        Debug.Log("Panoramik görüntü yakalanıyor...");
        
        // Panoramik görüntü yakala
        byte[] panoramaData = I360Render.Capture(panoramaWidth, captureAsJPEG, captureCamera, true);
        
        if (panoramaData != null)
        {
            SavePanoramaToFile(panoramaData);
            Debug.Log("Panoramik görüntü başarıyla yakalandı!");
        }
        else
        {
            Debug.LogError("Panoramik görüntü yakalanamadı!");
        }
    }
    
    private void SavePanoramaToFile(byte[] data)
    {
        string extension = captureAsJPEG ? ".jpg" : ".png";
        string filePath = Path.Combine(Application.persistentDataPath, fileName + extension);
        
        try
        {
            File.WriteAllBytes(filePath, data);
            Debug.Log($"Panorama kaydedildi: {filePath}");
            
            // Unity editöründe Resources klasörüne de kopyala
            #if UNITY_EDITOR
            SaveToResources(data, extension);
            #endif
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Dosya kaydedilemedi: {e.Message}");
        }
    }
    
    #if UNITY_EDITOR
    private void SaveToResources(byte[] data, string extension)
    {
        string resourcesPath = Path.Combine(Application.dataPath, "Resources");
        if (!Directory.Exists(resourcesPath))
        {
            Directory.CreateDirectory(resourcesPath);
        }
        
        string resourceFilePath = Path.Combine(resourcesPath, fileName + extension);
        File.WriteAllBytes(resourceFilePath, data);
        
        UnityEditor.AssetDatabase.Refresh();
        Debug.Log($"Panorama Resources klasörüne kaydedildi: {resourceFilePath}");
    }
    #endif
} 
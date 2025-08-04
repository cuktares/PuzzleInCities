using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionTracker : MonoBehaviour
{
    private List<(Vector3 position, Quaternion rotation)> recordedPositions = new List<(Vector3, Quaternion)>();
    private float recordInterval = 0.05f; // Dinamik olarak ayarlanacak
    private int maxPositions = 150; // Dinamik olarak ayarlanacak
    private bool isRecording = false;
    private Coroutine recordingCoroutine;

    private void Start()
    {
        // Platform bazlı optimizasyon ayarları
        SetPlatformOptimizations();
        StartRecording();
    }
    
    private void SetPlatformOptimizations()
    {
        // Mobil platformlar için ayarlama
        if (Application.isMobilePlatform)
        {
            // Mobilde biraz daha düşük ama yine de smooth olacak ayarlar
            recordInterval = 0.06f;
            maxPositions = 120;
        }
        else
        {
            // PC/Konsol için optimal ayarlar
            recordInterval = 0.04f;
            maxPositions = 180;
        }
        
        Debug.Log($"PositionTracker ayarları - Interval: {recordInterval}, Max Positions: {maxPositions}");
    }

    public void StartRecording()
    {
        if (!isRecording)
        {
            isRecording = true;
            if (recordingCoroutine != null)
            {
                StopCoroutine(recordingCoroutine);
            }
            recordingCoroutine = StartCoroutine(RecordPositionCoroutine());
        }
    }

    public void StopRecording()
    {
        if (isRecording)
        {
            isRecording = false;
            if (recordingCoroutine != null)
            {
                StopCoroutine(recordingCoroutine);
                recordingCoroutine = null;
            }
        }
    }

    private IEnumerator RecordPositionCoroutine()
    {
        while (isRecording)
        {
            RecordPosition();
            yield return new WaitForSeconds(recordInterval);
        }
    }

    private void RecordPosition()
    {
        if (!isRecording) return;

        // Sadece pozisyon değişmişse kaydet (optimize edilmiş)
        float minDistanceThreshold = Application.isMobilePlatform ? 0.05f : 0.03f;
        
        if (recordedPositions.Count == 0 || 
            Vector3.Distance(transform.position, recordedPositions[recordedPositions.Count - 1].position) > minDistanceThreshold)
        {
            recordedPositions.Add((transform.position, transform.rotation));
            
            if (recordedPositions.Count > maxPositions)
            {
                recordedPositions.RemoveAt(0);
            }
        }
    }

    public List<(Vector3 position, Quaternion rotation)> GetRecordedPositions()
    {
        return new List<(Vector3, Quaternion)>(recordedPositions);
    }

    public void ResetPositions()
    {
        recordedPositions.Clear();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (recordedPositions.Count == 0) return;

        Color startColor = new Color(1f, 0f, 0f, 0.3f);
        Color endColor = new Color(0f, 1f, 0f, 1f);

        for (int i = 0; i < recordedPositions.Count; i++)
        {
            float age = (float)i / (recordedPositions.Count - 1);
            Color positionColor = Color.Lerp(startColor, endColor, age);
            Gizmos.color = positionColor;

            Vector3 position = recordedPositions[i].position;
            
            // Sadece küçük küre çiz (daha optimize)
            Gizmos.DrawWireSphere(position, 0.1f);

            // Her 5. pozisyonda çizgi çiz (daha az çizgi)
            if (i < recordedPositions.Count - 1 && i % 5 == 0)
            {
                Gizmos.DrawLine(position, recordedPositions[i + 1].position);
            }
        }
    }
#endif

    private void OnDisable()
    {
        StopRecording();
    }
} 
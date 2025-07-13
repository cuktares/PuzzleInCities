using UnityEngine;

public class VehiclePatrol : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    [Tooltip("Araç Z ekseninde ne kadar mesafe git-gel yapacak (ileri-geri)")]
    public float patrolDistance = 10f;
    
    [Tooltip("Hareket hızı (m/s)")]
    public float moveSpeed = 3f;
    
    [Header("Yumuşak Hareket")]
    [Tooltip("Dönüş noktalarında ne kadar yumuşak durur")]
    public float smoothTurnTime = 0.5f;
    
    [Header("Debug")]
    public bool showDebugLines = true;
    
    // Private değişkenler
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private Vector3 pointA;
    private Vector3 pointB;
    private bool movingToB = true;
    private float journeyLength;
    private float journeyTime = 0f;
    private bool isMoving = true;
    
    void Start()
    {
        // Başlangıç pozisyonunu kaydet
        startPosition = transform.position;
        
        // Sadece Z ekseninde A ve B noktalarını hesapla
        pointA = new Vector3(
            startPosition.x, // X ekseni sabit
            startPosition.y, // Y ekseni sabit
            startPosition.z - (patrolDistance * 0.5f)  // Geri nokta
        );
        
        pointB = new Vector3(
            startPosition.x, // X ekseni sabit
            startPosition.y, // Y ekseni sabit
            startPosition.z + (patrolDistance * 0.5f)  // İleri nokta
        );
        
        // Mesafeyi hesapla
        journeyLength = patrolDistance;
        
        // İlk hedefi ayarla (B noktasına git)
        targetPosition = pointB;
        
        Debug.Log($"[{gameObject.name}] Z ekseni patrol başlatıldı. Geri: {pointA.z:F1}, İleri: {pointB.z:F1}");
    }
    
    void Update()
    {
        if (!isMoving) return;
        
        MoveVehicle();
    }
    
    void MoveVehicle()
    {
        // Sadece Z ekseninde mesafeyi kontrol et
        float distanceToTarget = Mathf.Abs(transform.position.z - targetPosition.z);
        
        // Hedefe çok yaklaştıysak yön değiştir
        if (distanceToTarget < 0.1f)
        {
            SwitchDirection();
        }
        
        // Sadece Z ekseninde hareket et
        float direction = (targetPosition.z > transform.position.z) ? 1f : -1f;
        
        // Yeni pozisyon hesapla - sadece Z ekseni değişecek
        Vector3 newPosition = transform.position;
        newPosition.z += direction * moveSpeed * Time.deltaTime;
        
        // X ve Y eksenlerini sabit tut
        newPosition.x = startPosition.x;
        newPosition.y = startPosition.y;
        
        // Pozisyonu uygula
        transform.position = newPosition;
        
        // Rotation hiç değişmez - araç hiç dönmez
    }
    
    void SwitchDirection()
    {
        // Yönü değiştir
        movingToB = !movingToB;
        targetPosition = movingToB ? pointB : pointA;
        
        string direction = movingToB ? "ileriye" : "geriye";
        Debug.Log($"[{gameObject.name}] Yön değiştirildi - {direction} gidiyor. Hedef Z: {targetPosition.z:F1}");
    }
    
    // Hareket durdurma/başlatma metodları
    public void StopPatrol()
    {
        isMoving = false;
        Debug.Log($"[{gameObject.name}] Patrol durduruldu");
    }
    
    public void StartPatrol()
    {
        isMoving = true;
        Debug.Log($"[{gameObject.name}] Patrol başlatıldı");
    }
    
    public void ResetPosition()
    {
        transform.position = startPosition;
        targetPosition = pointB;
        movingToB = true;
        Debug.Log($"[{gameObject.name}] Pozisyon sıfırlandı - Z ekseni hareket başlatıldı");
    }
    
    // Debug için Gizmos
    void OnDrawGizmos()
    {
        if (!showDebugLines) return;
        
        // Başlangıç pozisyonu yoksa runtime'da hesapla
        if (Application.isPlaying)
        {
            // Patrol hattı
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(pointA, pointB);
            
            // A ve B noktaları
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(pointA, 0.3f);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(pointB, 0.3f);
            
            // Mevcut hedef
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(targetPosition, 0.4f);
            
            // Hareket yönü
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, (targetPosition - transform.position).normalized * 2f);
        }
        else
        {
            // Editor modunda önizleme - sadece Z ekseni
            Vector3 previewStart = transform.position;
            
            Vector3 previewA = new Vector3(
                previewStart.x, // X ekseni sabit
                previewStart.y, // Y ekseni sabit
                previewStart.z - (patrolDistance * 0.5f)  // Geri nokta
            );
            
            Vector3 previewB = new Vector3(
                previewStart.x, // X ekseni sabit
                previewStart.y, // Y ekseni sabit
                previewStart.z + (patrolDistance * 0.5f)  // İleri nokta
            );
            
            Gizmos.color = Color.white;
            Gizmos.DrawLine(previewA, previewB);
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(previewA, 0.3f);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(previewB, 0.3f);
        }
    }
    
    void OnDrawGizmosSelected()
    {
        // Seçili olduğunda daha detaylı bilgi göster
        if (Application.isPlaying)
        {
            // Patrol bilgilerini konsola yazdır
            string direction = movingToB ? "ileriye" : "geriye";
            Debug.Log($"[{gameObject.name}] Patrol Bilgileri - Mesafe: {patrolDistance:F1}m, Hız: {moveSpeed:F1}, Yön: {direction}");
        }
    }
} 
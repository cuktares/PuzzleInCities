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
    
    [Tooltip("Hedefe yaklaşırken yavaşlama mesafesi")]
    public float slowDownDistance = 1f;
    
    [Header("Mobil Optimizasyon")]
    [Tooltip("Hedef mesafe toleransı (mobil için daha büyük değer)")]
    public float targetTolerance = 0.3f;
    
    [Tooltip("Maksimum hareket mesafesi per frame (titreme önleme)")]
    public float maxMoveDistancePerFrame = 0.5f;
    
    [Header("Debug")]
    public bool showDebugLines = true;
    
    // Private değişkenler
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private Vector3 pointA;
    private Vector3 pointB;
    private bool movingToB = true;
    private float journeyLength;
    private bool isMoving = true;
    
    // Yumuşak hareket için
    private float currentSpeed = 0f;
    private float turnTimer = 0f;
    private bool isInTurnPhase = false;
    
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
        
        if (isInTurnPhase)
        {
            HandleTurnPhase();
        }
        else
        {
            MoveVehicle();
        }
    }
    
    void MoveVehicle()
    {
        // Sadece Z ekseninde mesafeyi kontrol et
        float distanceToTarget = Mathf.Abs(transform.position.z - targetPosition.z);
        
        // Hedefe çok yaklaştıysak dönüş fazına geç
        if (distanceToTarget <= targetTolerance)
        {
            StartTurnPhase();
            return;
        }
        
        // Hedef yönünü hesapla
        float direction = (targetPosition.z > transform.position.z) ? 1f : -1f;
        
        // Hedefe yaklaşırken yavaşla
        float speedMultiplier = 1f;
        if (distanceToTarget < slowDownDistance)
        {
            speedMultiplier = Mathf.Clamp01(distanceToTarget / slowDownDistance);
            speedMultiplier = Mathf.Max(speedMultiplier, 0.2f); // Minimum hız
        }
        
        // Yumuşak hız geçişi
        float targetSpeed = moveSpeed * speedMultiplier;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * 5f);
        
        // Hareket mesafesini hesapla ve sınırla
        float moveDistance = currentSpeed * Time.deltaTime;
        moveDistance = Mathf.Min(moveDistance, maxMoveDistancePerFrame);
        
        // Hedef aşmayı önle
        if (moveDistance > distanceToTarget)
        {
            moveDistance = distanceToTarget;
        }
        
        // Yeni pozisyon hesapla - sadece Z ekseni değişecek
        Vector3 newPosition = transform.position;
        newPosition.z += direction * moveDistance;
        
        // X ve Y eksenlerini sabit tut
        newPosition.x = startPosition.x;
        newPosition.y = startPosition.y;
        
        // Pozisyonu uygula
        transform.position = newPosition;
    }
    
    void StartTurnPhase()
    {
        isInTurnPhase = true;
        turnTimer = 0f;
        currentSpeed = 0f;
        
        // Yönü değiştir
        movingToB = !movingToB;
        targetPosition = movingToB ? pointB : pointA;
        
        string direction = movingToB ? "ileriye" : "geriye";
        Debug.Log($"[{gameObject.name}] Dönüş fazı başladı - {direction} gidecek. Hedef Z: {targetPosition.z:F1}");
    }
    
    void HandleTurnPhase()
    {
        turnTimer += Time.deltaTime;
        
        // Dönüş süresi tamamlandıysa normal harekete dön
        if (turnTimer >= smoothTurnTime)
        {
            isInTurnPhase = false;
            turnTimer = 0f;
            
            string direction = movingToB ? "ileriye" : "geriye";
            Debug.Log($"[{gameObject.name}] Dönüş tamamlandı - {direction} hareket başlıyor");
        }
    }
    
    // Hareket durdurma/başlatma metodları
    public void StopPatrol()
    {
        isMoving = false;
        currentSpeed = 0f;
        isInTurnPhase = false;
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
        currentSpeed = 0f;
        isInTurnPhase = false;
        turnTimer = 0f;
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
            
            // Hedef tolerans alanı
            Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
            Gizmos.DrawSphere(targetPosition, targetTolerance);
            
            // Yavaşlama alanı
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.2f);
            Vector3 slowDownCenter = targetPosition;
            Gizmos.DrawSphere(slowDownCenter, slowDownDistance);
            
            // Hareket yönü
            if (!isInTurnPhase)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(transform.position, (targetPosition - transform.position).normalized * 2f);
            }
            
            // Dönüş fazı göstergesi
            if (isInTurnPhase)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawWireSphere(transform.position + Vector3.up * 2f, 0.5f);
            }
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
            string phase = isInTurnPhase ? "dönüş fazında" : "hareket halinde";
            Debug.Log($"[{gameObject.name}] Patrol Bilgileri - Mesafe: {patrolDistance:F1}m, Hız: {currentSpeed:F1}, Yön: {direction}, Durum: {phase}");
        }
    }
} 
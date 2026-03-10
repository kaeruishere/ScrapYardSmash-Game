using UnityEngine;
using System.Collections;

public class SimpleDebrisCleaner : MonoBehaviour
{
    public enum CleanupType { Sink, Shrink }

    [Header("Ayarlar")]
    public CleanupType cleanupMode = CleanupType.Shrink; // Buradan seçersin
    public float startDelay = 5.0f; // Patlamadan sonra ne kadar beklesin?
    public float animationDuration = 1.5f; // Batma veya küçülme ne kadar sürsün?

    [Header("Sink (Batma) Ayarı")]
    public float sinkDepth = 1.0f; // Ne kadar aşağı insin?

    [Header("Shrink (Küçülme) Ayarı")]
    public Vector3 targetScale = Vector3.zero; // Kaça kadar küçülsün?

    private Vector3 initialScale;
    private Rigidbody[] bodies;
    private Collider[] colliders;

    void Awake()
    {
        initialScale = transform.localScale;
        bodies = GetComponentsInChildren<Rigidbody>();
        colliders = GetComponentsInChildren<Collider>();
    }

    void OnEnable()
    {
        StartCoroutine(ProcessCleanup());
    }

    private IEnumerator ProcessCleanup()
    {
        // 1. Bekleme Süresi
        yield return new WaitForSeconds(startDelay);

        // 2. Fizikleri Kapat (Dondur)
        foreach (var rb in bodies)
        {
            rb.isKinematic = true; // Hareketi kes
            rb.detectCollisions = false; // Çarpışmayı kes (Performans+)
        }
        
        foreach (var col in colliders)
        {
            col.enabled = false; // Collider'ları kapat
        }

        // 3. Seçilen Animasyonu Yap
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Vector3 startScale = transform.localScale;
        Vector3 endPos = startPos - Vector3.up * sinkDepth;

        while (elapsed < animationDuration)
        {
            float t = elapsed / animationDuration;
            float smoothT = t * t * (3f - 2f * t); // SmoothStep formülü (Matematiksel olarak daha ucuz)

            if (cleanupMode == CleanupType.Sink)
            {
                // Sadece pozisyonu aşağı çek
                transform.position = Vector3.Lerp(startPos, endPos, smoothT);
            }
            else // Shrink
            {
                // Sadece scale'i küçült
                transform.localScale = Vector3.Lerp(startScale, targetScale, smoothT);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 4. Yok et
        Destroy(gameObject);
    }
}
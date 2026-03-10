using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeleeWeaponPhysics : WeaponBase
{
    [Header("Fiziksel Ayarlar")]
    public float minDamageVelocity = 2.0f; 
    public float damageMultiplier = 1.5f;  
    public float maxBonusDamage = 30f;     
    
    [Header("Süre Ayarları")]
    public float activeDuration = 0.5f; // Collider'ın açık kalacağı süre

    private bool isAttacking = false;
    private Vector3 lastPosition;
    private float currentSpeed;
    private Collider myCollider;

    private List<GameObject> hitObjects = new List<GameObject>();

    private void Start()
    {
        myCollider = GetComponent<Collider>();

        if (myCollider != null)
        {
            // Ayarları yap ama BAŞLANGIÇTA KAPAT
            myCollider.isTrigger = true; 
            
            if (myCollider is MeshCollider meshCol) 
                meshCol.convex = true;

            // --- DEĞİŞİKLİK BURADA: Oyuna başlarken collider kapalı olsun ---
            myCollider.enabled = false; 
        }
        else
        {
            Debug.LogError("SİLAHTA COLLIDER YOK!");
        }

        IgnorePlayerCollision();
        lastPosition = transform.position;
    }

    private void FixedUpdate()
    {
        // Hız hesabı her zaman çalışsın (Saldırı anında doğru hızı bulmak için)
        float distance = (transform.position - lastPosition).magnitude;
        currentSpeed = distance / Time.fixedDeltaTime;
        lastPosition = transform.position;
    }

    protected override void PerformAttack()
    {
        // Eğer zaten saldırıyorsak tekrar başlatma
        if (!isAttacking)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        hitObjects.Clear(); 
        
        // --- 1. SALDIRI BAŞLADI: COLLIDER'I AÇ ---
        if (myCollider != null) myCollider.enabled = true;
        
        Debug.Log("--- SALDIRI BAŞLADI (Collider AÇIK) ---");
        
        // Animasyon süresi kadar bekle (Bu sürede collider açık kalır)
        yield return new WaitForSeconds(activeDuration);
        
        // --- 2. SALDIRI BİTTİ: COLLIDER'I KAPAT ---
        if (myCollider != null) myCollider.enabled = false;
        
        isAttacking = false;
        Debug.Log("--- SALDIRI BİTTİ (Collider KAPALI) ---");
    }

    private void OnTriggerEnter(Collider other)
    {
        // Collider kapalıyken burası çalışmaz ama biz yine de listeyi kontrol edelim
        if (hitObjects.Contains(other.gameObject)) return;

        if (other.CompareTag("Player") || other.gameObject == this.gameObject) return;

        // Hız Kontrolü
        if (currentSpeed < minDamageVelocity)
        {
            // Yavaş vurunca log kirliliği yapmasın diye commentledim, istersen açarsın
            // Debug.Log($"Hız Yetersiz: {currentSpeed}");
            return;
        }

        float bonusDamage = Mathf.Clamp((currentSpeed * damageMultiplier), 0, maxBonusDamage);
        float totalDamage = damage + bonusDamage;

        DestructibleObject target = other.GetComponentInParent<DestructibleObject>();
        
        if (target != null)
        {
            hitObjects.Add(other.gameObject); // Vurduğumuzu not et
            target.TakeDamage(totalDamage, DamageType.Impact);
            Debug.Log($"<color=green>HASAR:</color> {totalDamage} (Hız: {currentSpeed:F1})");
        }
        
        // Fiziksel İtme
        Rigidbody targetRb = other.GetComponent<Rigidbody>();
        if (targetRb != null)
        {
            Vector3 forceDir = (other.transform.position - transform.position).normalized;
            targetRb.AddForce(forceDir * totalDamage * 0.5f, ForceMode.Impulse);
        }
    }

    private void IgnorePlayerCollision()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && myCollider != null)
        {
            Collider[] playerColliders = player.GetComponentsInChildren<Collider>();
            foreach (var col in playerColliders)
            {
                if(col != myCollider) Physics.IgnoreCollision(myCollider, col, true);
            }
        }
    }
}
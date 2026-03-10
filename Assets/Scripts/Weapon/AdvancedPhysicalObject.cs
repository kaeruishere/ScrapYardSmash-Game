using UnityEngine;
using System;
using System.Collections.Generic;

public class AdvancedPhysicalObject : MonoBehaviour
{
    #region --- YENİ EKLENEN FİZİK AYARLARI ---
    [Header("--- TAŞIMA & AĞIRLIK ---")]
    public bool isPickupable = true;
    public bool isThrowable = true;

    [Tooltip("Ağırlık Çarpanı: 1 = Normal. 2 = İki kat ağır.")]
    [Range(0.1f, 5f)]
    public float weightMultiplier = 2.5f;
    #endregion

    #region --- AYARLAR: SAĞLIK & KIRILMA ---
    [Header("--- HEALTH SETTINGS ---")]
    public bool isDestructible = true;
    public bool takesSelfImpactDamage = true;
    [SerializeField] private float maxHealth = 100f;
    public float currentHealth { get; private set; }

    [Header("--- DEBRIS ---")]
    [SerializeField] private GameObject debrisRoot;
    [SerializeField] private float explosionPower = 2f;
    
    [Header("--- SCORE ---")]
    public int scoreValue = 10;
    #endregion

    #region --- AYARLAR: SİLAH & HASAR VERME ---
    [Header("--- WEAPON SETTINGS ---")]
    public float baseDamage = 10f;
    public float impactDamageMultiplier = 1.0f;
    public float selfDamageMultiplier = 0.5f;

    [Header("--- THRESHOLDS ---")]
    public float minImpactSpeed = 1.0f;   
    public float minImpactForce = 2.0f;   
    #endregion

    private Rigidbody rb;
    private float ignoreCollisionTime = 0f;
    public bool isThrown = false;
    
    public event Action<float, float> OnHealthChanged;
    public event Action OnDeath;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        currentHealth = maxHealth;
        
        if (rb != null)
        {
            rb.mass *= weightMultiplier; 
        }
    }

    void Start()
    {
        if (debrisRoot != null) debrisRoot.SetActive(false);
    }

    void Update()
    {
        if (ignoreCollisionTime > 0f)
            ignoreCollisionTime -= Time.deltaTime;
    }

    // --- HASAR ALMA SİSTEMİ ---
    public void TakeDamage(float amount)
    {
        if (!isDestructible || currentHealth <= 0) return;

        Debug.Log($"🩸 {name} Hasar Aldı! Miktar: {amount}. Kalan Can: {currentHealth - amount}");

        currentHealth -= amount;
        OnHealthChanged?.Invoke(currentHealth, maxHealth); 

        if (currentHealth <= 0) Die();
    }
    
    private void Die()
    {
        Debug.Log($"💀 {name} ÖLDÜ/KIRILDI!");
        OnDeath?.Invoke();
        if (GameManager.Instance != null) GameManager.Instance.AddScore(scoreValue);
        SpawnDebris();
    }

    private void SpawnDebris()
    {
        if (debrisRoot == null) 
        { 
            Destroy(gameObject); 
            return; 
        }
        
        debrisRoot.SetActive(true);
        debrisRoot.transform.SetParent(null);
        foreach (var piece in debrisRoot.GetComponentsInChildren<Rigidbody>())
        {
            piece.isKinematic = false;
            // Unity sürümüne göre 'velocity' veya 'linearVelocity' kullan
            #if UNITY_6000_0_OR_NEWER
            piece.linearVelocity = rb.linearVelocity;
            #else
            piece.velocity = rb.velocity;
            #endif
            
            piece.AddExplosionForce(explosionPower, transform.position, 1.5f, 0.15f, ForceMode.Impulse);
        }
        Destroy(gameObject);
    }

    // --- ÇARPIŞMA SİSTEMİ (DÜZELTİLDİ) ---
    private void OnCollisionEnter(Collision collision)
    {
        // 1. Eğer fırlatıldıktan sonraki koruma süresindeyse çarpışmayı yok say
        if (ignoreCollisionTime > 0f) return;

        // NOT: "!isThrown" kontrolünü kaldırdım. Artık yere düşse de hasar alır.
        
        // 2. HIZ KONTROLÜ
        // Unity 6 kullanıyorsan 'linearVelocity', eskiyse 'velocity'
        #if UNITY_6000_0_OR_NEWER
        float speed = rb.linearVelocity.magnitude;
        #else
        float speed = rb.velocity.magnitude;
        #endif

        // LOG: Hız kontrolü
        // Debug.Log($"ℹ️ Çarpışma Hızı: {speed} (Min: {minImpactSpeed}) | Obje: {name}");

        if (speed < minImpactSpeed) return;

        // 3. GÜÇ KONTROLÜ
        float impactForce = speed * rb.mass;
        
        // LOG: Güç kontrolü
        // Debug.Log($"💪 Çarpışma Gücü: {impactForce} (Min: {minImpactForce})");

        if (impactForce < minImpactForce) return;

        // 4. HASAR HESAPLAMA
        float outgoingDamage = baseDamage + (impactForce * impactDamageMultiplier);
        float selfDamage = (baseDamage + impactForce) * selfDamageMultiplier;

        Debug.Log($"💥 ÇARPIŞMA OLDU! {name} -> {collision.collider.name} | Giden Hasar: {outgoingDamage} | Kendine Hasar: {selfDamage}");

        // --- HEDEFE HASAR VER ---
        AdvancedPhysicalObject target = collision.collider.GetComponentInParent<AdvancedPhysicalObject>();
        if (target != null)
        {
            target.TakeDamage(outgoingDamage);
        }

        // --- KENDİNE HASAR VER ---
        // Sadece fırlatılınca değil, her sert çarpışmada hasar alsın istiyorsan burası çalışmalı
        if (takesSelfImpactDamage && isDestructible)
        {
            TakeDamage(selfDamage);
        }

        // Çarpışma bitince fırlatılma durumunu sıfırla
        isThrown = false; 
    }
    
    public void MarkAsThrown() { isThrown = true; ignoreCollisionTime = 0.1f; }
    public void ResetPickupState() { isThrown = false; ignoreCollisionTime = 0.1f; }
}
using UnityEngine;
using System;

public enum DamageType { Generic, Ballistic, Impact, Explosive }

public class DestructibleObject : MonoBehaviour
{
    [Header("Skor Ayarı")]
    [Tooltip("Bu obje kırılınca oyuncu kaç puan kazansın?")]
    public int scoreValue = 50; 

    [Header("Dayanıklılık")]
    public float maxHealth = 100f;
    protected float currentHealth;

    [Header("Hasar Çarpanları (Dirençler)")]
    [Tooltip("Mermi hasarını neyle çarpalım? (0 = Hasar Almaz, 2 = Çift Hasar Alır)")]
    public float ballisticMultiplier = 1.0f; 
    
    [Tooltip("Sopa/Darbe hasarını neyle çarpalım?")]
    public float impactMultiplier = 1.0f;    
    
    [Header("Kırılma Ayarları")]
    public GameObject brokenVersionPrefab;
    public bool spawnDebrisFromPool = false;
    public string debrisPoolTag = "GlassShards";
    
    [Header("Çarpışma Fiziği")]
    public float minBreakForce = 5f;
    public float collisionDamageMultiplier = 1f;



    [Header("Parçalanma Fiziği")]
    [Tooltip("Parçalar ne kadar sert fırlasın? (Küçük objeler için 0.5 - 2 arası ideal)")]
    public float debrisExplosionForce = 1f; 

    [Tooltip("Parçalar ne kadar alana yayılsın? (Radius)")]
    public float explosionRadius = 2f;

    
    private bool isBroken = false;

    void Start() 
    { 
        currentHealth = maxHealth; 
        Debug.Log($"<color=cyan>[BAŞLATILDI]</color> {name} hazır. Can: {currentHealth}");
    }

    public virtual void TakeDamage(float amount, DamageType type = DamageType.Generic)
    {
        if (isBroken) 
        {
            Debug.LogWarning($"[DURUM] {name} zaten kırık! Gelen hasar iptal edildi.");
            return;
        }

        Debug.Log($"<color=yellow>[HASAR GİRİŞİ]</color> {name} objesine {amount} miktarında {type} hasarı geldi.");

        float finalDamage = amount;
        float appliedMultiplier = 1.0f;

        switch (type)
        {
            case DamageType.Ballistic:
                finalDamage *= ballisticMultiplier;
                appliedMultiplier = ballisticMultiplier;
                break;
            case DamageType.Impact:
                finalDamage *= impactMultiplier;
                appliedMultiplier = impactMultiplier;
                break;
            case DamageType.Explosive:
                finalDamage *= 1.0f; 
                break;
        }

        if (Math.Abs(appliedMultiplier - 1.0f) > 0.01f)
        {
            Debug.Log($" -> Çarpan Uygulandı ({type}): x{appliedMultiplier}. Yeni Hasar: {finalDamage}");
        }

        if (finalDamage <= 0) 
        {
            Debug.Log($"<color=grey>[ABSORBE]</color> {name} hasarı tamamen engelledi.");
            return;
        }

        currentHealth -= finalDamage;
        Debug.Log($"<color=red>[HASAR SONUCU]</color> {name} -{finalDamage} Can kaybetti. Kalan Can: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0) 
        {
            Debug.Log($"<color=red><b>[KIRILMA]</b></color> {name} canı bitti! Kırılıyor...");
            Break();
        }
    }

    protected virtual void Break()
    {
        if (isBroken) return;
        isBroken = true;
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnObjectBroken(scoreValue);
        }

        Debug.Log($"[İŞLEM] {name} parçalanma efekti (Debris) oluşturuluyor.");
        SpawnDebris();
        
        Debug.Log($"[YOK OLMA] {name} sahneden siliniyor.");
        Destroy(gameObject); 
    }

    private void SpawnDebris()
    {
        GameObject debris = null;

        if (spawnDebrisFromPool && ObjectPooler.Instance != null)
            debris = ObjectPooler.Instance.SpawnFromPool(debrisPoolTag, transform.position, transform.rotation);
        else if (brokenVersionPrefab != null)
            debris = Instantiate(brokenVersionPrefab, transform.position, transform.rotation);

        if (debris != null)
        {
            foreach (var rb in debris.GetComponentsInChildren<Rigidbody>())
            {
                rb.AddExplosionForce(debrisExplosionForce, transform.position, explosionRadius, 0.5f, ForceMode.Impulse);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isBroken) return;

        float impactForce = collision.relativeVelocity.magnitude;
        string colliderName = collision.gameObject.name;

      
        Debug.Log($"[ÇARPIŞMA] {name} <-> {colliderName} | Şiddet: {impactForce:F2}");

        if (impactForce >= minBreakForce)
        {
            float massFactor = 1f;
            if (collision.rigidbody) massFactor = collision.rigidbody.mass;

            float calculatedDamage = impactForce * massFactor * collisionDamageMultiplier;
            
            Debug.Log($"<color=orange>[SERT ÇARPIŞMA]</color> {colliderName} çarptı! " +
                      $"Hız: {impactForce:F1} * Kütle: {massFactor} * Çarpan: {collisionDamageMultiplier} " +
                      $"= Hasar: {calculatedDamage:F1}");

            TakeDamage(calculatedDamage, DamageType.Impact);
        }
        else if (impactForce > 1f)
        {
            Debug.Log($"[HAFİF ÇARPIŞMA] {colliderName} çarptı ama yetersiz güç ({impactForce:F1} < {minBreakForce}). Hasar yok.");
        }
    }
}
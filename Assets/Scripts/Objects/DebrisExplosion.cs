using UnityEngine;

public class DebrisExplosion : MonoBehaviour
{
    [Header("Görsel & Ses")]
    public GameObject explosionVFX;      // Patlama partikülü
    public AudioClip explosionSound;     // Patlama sesi
    public float soundVolume = 1f;

    [Header("Fizik & Hasar")]
    public float explosionForce = 1000f; // Şarapnellerin fırlama gücü
    public float explosionRadius = 5f;   // Etki alanı
    public float damageAmount = 100f;    // Düşmana verilecek hasar
    public LayerMask damageLayers;       // Kimlere hasar verilsin?

    void Start()
    {
        // Sahneye oluşur oluşmaz patlat!
        Explode();
    }

    void Explode()
    {
        // 1. Görsel Efekti Oluştur
        if (explosionVFX != null)
        {
            Instantiate(explosionVFX, transform.position, transform.rotation);
        }

        // 2. Sesi Çal (PlayClipAtPoint, obje yok olsa bile sesin bitmesini sağlar)
        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, transform.position, soundVolume);
        }

        // 3. Şarapnelleri (Kendi Çocuklarını) Fırlat
        Rigidbody[] shards = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody shard in shards)
        {
            shard.AddExplosionForce(explosionForce, transform.position, explosionRadius);
        }

        // 4. Etraftaki Düşmanlara/Varillere Hasar Ver ve İt
        ApplyAreaDamage();
    }

    void ApplyAreaDamage()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius, damageLayers);

        foreach (Collider hit in hits)
        {
            // Kendimize (Şarapnele) hasar vermeyelim
            if (hit.transform.root == transform) continue;

            // A. Fiziksel İtme (Düşman veya Kutu)
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce * 1.5f, transform.position, explosionRadius, 1f, ForceMode.Impulse);
            }

            // B. Hasar Verme (DestructibleObject veya Enemy)
            DestructibleObject destObj = hit.GetComponent<DestructibleObject>();
            if (destObj != null)
            {
                // Zincirleme reaksiyon için Explosive tipi hasar
                destObj.TakeDamage(damageAmount, DamageType.Explosive);
            }

            // Eğer Enemy scriptin varsa buraya ekle:
            // if (hit.TryGetComponent(out EnemyHealth enemy)) enemy.TakeDamage(damageAmount);
        }
    }
}
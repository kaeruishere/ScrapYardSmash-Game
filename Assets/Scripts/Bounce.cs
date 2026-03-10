using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bounce : MonoBehaviour
{
    [Header("Bounce Settings")]
    [Tooltip("Sekme katsayısı (1 = hiç enerji kaybetme, 0.8 = gerçekçi basket topu)")]
    public float bounciness = 0.8f;

    [Tooltip("Bu hızdan yavaşsa artık sekmesin")]
    public float minBounceSpeed = 1f;

    [Tooltip("Sekmeden sonra çıkabileceği maksimum hız")]
    public float maxBounceSpeed = 12f;

    [Header("Energy Loss")]
    [Tooltip("Her sekmede kalan enerji oranı")]
    public float energyLossPerBounce = 0.9f;

    [Header("Ground Filter")]
    [Tooltip("Sadece bu tag'e sahip collider'larda sekme hesabı yap")]
    public string groundTag = "Ground";

    private Rigidbody rb;
    private Vector3 lastVelocity;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Fizik updateleriyle aynı frame'de hız kaydedilsin
        lastVelocity = rb.linearVelocity;
    }

    void OnCollisionEnter(Collision collision)
    {
        // İstersen sadece zeminde sekme olsun
        if (!string.IsNullOrEmpty(groundTag) && !collision.collider.CompareTag(groundTag))
            return;

        float speed = lastVelocity.magnitude;
        if (speed < minBounceSpeed)
            return;

        // İlk temas noktasının normalini al
        Vector3 normal = collision.contacts[0].normal;

        // Neredeyse düz zemin değilse (duvar, eğik yüzey vs.), bounce yapma
        if (normal.y < 0.5f)
            return;

        // Mevcut hız vektörünü dikey + yatay bileşenlere ayıralım
        Vector3 velocity = lastVelocity;

        // Normal (genelde (0,1,0)) yönündeki bileşen (dikey)
        Vector3 verticalComponent = Vector3.Project(velocity, normal);
        // Geri kalanı yatay bileşen
        Vector3 horizontalComponent = velocity - verticalComponent;

        // Dikey bileşeni ters çevir (sekme) + enerji/bounce katsayısı uygula
        Vector3 newVerticalComponent = -verticalComponent * bounciness * energyLossPerBounce;

        // Toplam yeni hız
        Vector3 newVelocity = horizontalComponent + newVerticalComponent;

        // Limit uygula
        newVelocity = Vector3.ClampMagnitude(newVelocity, maxBounceSpeed);

        rb.linearVelocity = newVelocity;
    }
}

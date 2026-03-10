using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BasketballPhysics : MonoBehaviour
{
    [Header("Sekme (Bounce) Ayarları")]
    public float bounciness = 0.8f;
    public float minBounceSpeed = 1f;
    public float maxBounceSpeed = 12f;
    public float energyLossPerBounce = 0.9f;
    public string groundTag = "Ground";

    private Rigidbody rb;
    private Vector3 lastVelocity;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Havada süzülürken hızını kaydet (Sekme hesaplaması için)
        // Unity 6+ ise rb.linearVelocity kullan, yoksa rb.velocity
        lastVelocity = rb.linearVelocity;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Eğer top "Kinematic" ise (yani karakterin elindeyse) sekme hesabı yapma
        if (rb.isKinematic) return;

        // Tag kontrolü
        if (!string.IsNullOrEmpty(groundTag) && !collision.collider.CompareTag(groundTag))
            return;

        float speed = lastVelocity.magnitude;
        if (speed < minBounceSpeed) return;

        Vector3 normal = collision.contacts[0].normal;
        if (normal.y < 0.5f) return;

        // Sekme Formülü (Senin kodun)
        Vector3 velocity = lastVelocity;
        Vector3 verticalComponent = Vector3.Project(velocity, normal);
        Vector3 horizontalComponent = velocity - verticalComponent;
        Vector3 newVerticalComponent = -verticalComponent * bounciness * energyLossPerBounce;
        Vector3 newVelocity = horizontalComponent + newVerticalComponent;

        // Limit
        newVelocity = Vector3.ClampMagnitude(newVelocity, maxBounceSpeed);

        rb.linearVelocity = newVelocity;
    }
}
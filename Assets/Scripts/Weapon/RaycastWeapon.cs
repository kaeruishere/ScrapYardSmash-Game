using UnityEngine;
using System.Collections;

public class RaycastWeapon : WeaponBase
{
    public int maxAmmo = 6;
    public float range = 50f;
    public float reloadTime = 1.5f;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;
    
    private int currentAmmo;
    private bool isReloading = false;

    void Start() { currentAmmo = maxAmmo; }

    protected override bool CanAttack() { return !isReloading && currentAmmo > 0; }

    protected override void PerformAttack()
    {
        currentAmmo--;
        if (muzzleFlash) muzzleFlash.Play();

        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, range))
        {
            DestructibleObject target = hit.collider.GetComponentInParent<DestructibleObject>();
            if (target != null) target.TakeDamage(damage, DamageType.Ballistic);

            if (impactEffect) Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
            if (rb) rb.AddForce(-hit.normal * 5f, ForceMode.Impulse);
        }
    }

    public override void Reload()
    {
        if (!isReloading && currentAmmo < maxAmmo) StartCoroutine(ReloadRoutine());
    }

    IEnumerator ReloadRoutine()
    {
        isReloading = true;
        if (weaponAnimator) weaponAnimator.SetTrigger("Reload");
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        isReloading = false;
    }
}
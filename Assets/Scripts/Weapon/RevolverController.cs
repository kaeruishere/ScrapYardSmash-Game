using UnityEngine;
using System.Collections;

public class RevolverController : MonoBehaviour
{
    [Header("Silah Verileri")]
    public float damage = 25f;
    public float range = 100f;
    public float fireRate = 0.5f; 
    
    [Header("Mermi ve Reload")]
    public int maxAmmo = 6;
    public int currentAmmo;
    public float reloadTime = 1.5f;
    public bool isReloading = false;

    [Header("Efektler")]
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect; 
    public AudioClip fireSound;
    private AudioSource audioSource;
    public Animator gunAnimator; 

    [HideInInspector] public float nextFireTime = 0f;

    void Start()
    {
        currentAmmo = maxAmmo;
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
    }

    public bool CanFire()
    {
        if (isReloading) return false;
        if (currentAmmo <= 0) return false;
        if (Time.time < nextFireTime) return false;
        return true;
    }

    public void FireVisuals()
    {
        nextFireTime = Time.time + fireRate;
        currentAmmo--;

        if (audioSource && fireSound)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(fireSound);
        }

        if (muzzleFlash) muzzleFlash.Play();

        if (gunAnimator) gunAnimator.SetTrigger("Fire");
    }

    public void StartReload()
    {
        if (isReloading || currentAmmo >= maxAmmo) return;
        StartCoroutine(ReloadRoutine());
    }

    IEnumerator ReloadRoutine()
    {
        isReloading = true;
        if (gunAnimator) gunAnimator.SetTrigger("Reload");

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        isReloading = false;
    }
}
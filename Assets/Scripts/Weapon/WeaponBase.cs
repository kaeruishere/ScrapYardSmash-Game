using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [Header("Genel Silah Ayarları")]
    public string weaponName;
    public float damage = 10f;
    public float fireRate = 0.5f;

    [Header("Animasyon Ayarları")]
    public Animator weaponAnimator; 
    protected Animator playerAnimator; 

    public string playerAttackAnimTrigger = "Fire"; 

    [Header("Ses")]
    public AudioSource audioSource;
    public AudioClip attackSound;

    protected float nextAttackTime = 0f;
    protected Camera playerCamera;
    public virtual void Initialize(Camera cameraRef, Animator playerAnimRef)
    {
        playerCamera = cameraRef;
        playerAnimator = playerAnimRef;
        
        if(audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    public void TryAttack()
    {
        if (Time.time >= nextAttackTime && CanAttack())
        {
            nextAttackTime = Time.time + fireRate;
            PerformAttack();
            PlayEffects();
        }
        else if (Time.time >= nextAttackTime)
        {
            OnAttackFailed();
        }
    }

    public virtual void Reload() { }
    protected abstract void PerformAttack();
    protected virtual bool CanAttack() { return true; }
    protected virtual void OnAttackFailed() { }

    protected void PlayEffects()
    {
        if (weaponAnimator) weaponAnimator.SetTrigger("Fire");
        if (playerAnimator) playerAnimator.SetTrigger(playerAttackAnimTrigger);
        if (audioSource && attackSound) audioSource.PlayOneShot(attackSound);
    }
}
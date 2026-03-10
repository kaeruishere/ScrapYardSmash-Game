using UnityEngine;
using System.Collections;

public class ExplosiveBarrel : DestructibleObject
{
    [Header("Fitil Ayarları")]
    public float fuseTime = 2.5f;
    
    [Header("Fitil Efektleri")]
    public GameObject fireVFX;
    public AudioClip fuseSound;

    private bool isFuseLit = false;

    public override void TakeDamage(float amount, DamageType type = DamageType.Generic)
    {
        if (isFuseLit)
        {
            base.Break();
        }
        else
        {
            StartCoroutine(StartFuse());
        }
    }

    IEnumerator StartFuse()
    {
        isFuseLit = true;
        Debug.Log("Fitil ateşlendi...");

        if (fireVFX != null)
        {
            GameObject fire = Instantiate(fireVFX, transform.position, Quaternion.identity);
            fire.transform.SetParent(this.transform);
        }

        if (fuseSound != null)
        {
            AudioSource.PlayClipAtPoint(fuseSound, transform.position);
        }

        yield return new WaitForSeconds(fuseTime);
        base.Break(); 
    }
}
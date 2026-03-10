using UnityEngine;

[RequireComponent(typeof(AudioSource))] // Otomatik AudioSource ekler
public class RandomSoundPlayer : MonoBehaviour
{
    [Header("Ses Listesi")]
    [Tooltip("Buraya çarpma/kırılma seslerini sürükle")]
    public AudioClip[] clips;

    [Header("Ayarlar")]
    [Range(0f, 1f)] public float volume = 1f;
    
    [Tooltip("Sesin hep aynı tonda çıkmasını engeller")]
    public bool randomizePitch = true;
    public float minPitch = 0.8f;
    public float maxPitch = 1.2f;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Eğer listede ses varsa çal
        if (clips.Length > 0)
        {
            PlayRandomSound();
        }
    }

    void PlayRandomSound()
    {
        // 1. Rastgele bir ses seç
        int randomIndex = Random.Range(0, clips.Length);
        audioSource.clip = clips[randomIndex];

        // 2. Ses ayarlarını yap
        audioSource.volume = volume;
        audioSource.spatialBlend = 1f; // 3D Ses yap (Ses objeden gelsin)
        
        // Sesin ne kadar uzaktan duyulacağını ayarlar (Opsiyonel)
        audioSource.minDistance = 2f;
        audioSource.maxDistance = 20f;

        // 3. Pitch (Ton) rastgeleleştirme
        if (randomizePitch)
        {
            audioSource.pitch = Random.Range(minPitch, maxPitch);
        }

        // 4. Çal
        audioSource.Play();
    }
}
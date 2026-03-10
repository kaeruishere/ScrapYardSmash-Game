using System.Collections;
using UnityEngine;

public class TrapMachine : MonoBehaviour
{
    [Header("Ayarlar")]
    public float atisSikligi = 2.0f; // Kaç saniyede bir fırlatsın?
    public float ilkAtisGecikmesi = 0f; // Açıldıktan kaç saniye sonra ilk atış?
    public bool sonsuzDongu = true; // Sürekli atsın mı?
    public int atisSayisi = 5; // Sonsuz değilse kaç tane atsın?
    public float ileriGuc = 15f;     // İleri doğru fırlatma gücü
    public float yukariGuc = 5f;     // Yukarı doğru fırlatma gücü

    [Header("Referanslar")]
    public GameObject projectilePrefab; // Fırlatılacak nesne (Disk/Tabak)
    public Transform spawnPoint;        // Nereden çıkacak?
    public Renderer lampRenderer;       // Renk değiştirecek lamba objesi

    [Header("Renkler")]
    public Color kapaliRenk = Color.red;
    public Color acikRenk = Color.green;

    private bool isRunning = false;
    private Coroutine firingCoroutine;

    void Start()
    {
        // Oyuna başlarken lambayı kırmızı yap
        UpdateLampColor(kapaliRenk);
    }

    // Bu fonksiyonu dışarıdan (Raycast ile veya Player scriptinden) çağıracağız
    public void ToggleMachine()
    {
        isRunning = !isRunning;

        if (isRunning)
        {
            // Makineyi AÇ
            UpdateLampColor(acikRenk);
            firingCoroutine = StartCoroutine(FireRoutine());
        }
        else
        {
            // Makineyi KAPAT
            UpdateLampColor(kapaliRenk);
            if (firingCoroutine != null)
            {
                StopCoroutine(firingCoroutine);
            }
        }
    }

    void UpdateLampColor(Color color)
    {
        if (lampRenderer != null)
        {
            // Normal renk değişimi
            lampRenderer.material.color = color;
            
            // Eğer "Emission" kullanarak parlamasını istiyorsan (HDR):
            lampRenderer.material.SetColor("_EmissionColor", color * 2f); 
            // Not: Emission için materyalde "Enable Keyword" ayarı gerekebilir.
        }
    }

    IEnumerator FireRoutine()
    {
        // İlk gecikme
        if (ilkAtisGecikmesi > 0)
        {
            yield return new WaitForSeconds(ilkAtisGecikmesi);
        }

        if (!isRunning) yield break; // Gecikme sırasında kapatıldıysa çık

        if (sonsuzDongu)
        {
            while (isRunning)
            {
                FireProjectile();
                yield return new WaitForSeconds(atisSikligi);
            }
        }
        else
        {
            for (int i = 0; i < atisSayisi; i++)
            {
                if (!isRunning) break;
                FireProjectile();
                if (i < atisSayisi - 1) yield return new WaitForSeconds(atisSikligi);
            }
            
            // İş bitti, makineyi kapat
            if (isRunning)
            {
                isRunning = false;
                UpdateLampColor(kapaliRenk);
            }
        }
    }

    void FireProjectile()
    {
        if (projectilePrefab == null || spawnPoint == null) return;

        // 1. Prefabı oluştur
        GameObject clone = Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation);

        // 2. Rigidbody'yi al
        Rigidbody rb = clone.GetComponent<Rigidbody>();

        if (rb != null)
        {
            // 3. Kuvvet uygula (Local forward + Local up)
            // spawnPoint.forward: Noktanın baktığı yön (Mavi ok)
            // spawnPoint.up: Noktanın yukarısı (Yeşil ok)
            Vector3 force = (spawnPoint.forward * ileriGuc) + (spawnPoint.up * yukariGuc);
            
            rb.AddForce(force, ForceMode.Impulse); // Impulse: Ani vuruş hissi verir
            
            // Opsiyonel: Rastgele dönüş ekle (disk havada dönsün diye)
            rb.AddTorque(Random.insideUnitSphere * 10f, ForceMode.Impulse);
        }
    }
}
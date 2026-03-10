using UnityEngine;

public class UpperBodyIK : MonoBehaviour
{
    [Header("Ayarlar")]
    public Transform chestBone;       // Karakterin Göğüs/Spine kemiği
    public Transform cameraTransform; // Oyuncunun kamerası
    
    [Header("Sınırlamalar")]
    public float minAngle = -45f;     // En fazla ne kadar aşağı baksın
    public float maxAngle = 45f;      // En fazla ne kadar yukarı baksın
    public Vector3 offsetRotation;    // Eğer yamuk duruyorsa buradan ayarla (örn: 0, 0, 90)

    // LateUpdate: Unity animasyonu oynatır, hemen ardından biz kemiği bükeriz.
    void LateUpdate()
    {
        if (chestBone == null || cameraTransform == null) return;

        // 1. Kameranın X eksenindeki açısını (yukarı/aşağı bakma) al
        float lookAngle = cameraTransform.localEulerAngles.x;

        // 2. Açıyı 0-360 formatından -180/+180 formatına çevir (Unity bazen 350 derece der, o aslında -10'dur)
        if (lookAngle > 180) lookAngle -= 360;

        // 3. Açıyı sınırla (Karakterin beli kırılmasın)
        lookAngle = Mathf.Clamp(lookAngle, minAngle, maxAngle);

        // 4. Bu açıyı Göğüs kemiğine uygula.
        // NOT: Senin modeline göre "Vector3.right" (Kırmızı ok) yönü değişebilir.
        // Eğer karakter yana yatarsa buradaki (lookAngle, 0, 0) yerlerini değiştirmen gerekir.
        
        // Mevcut rotasyonu alıp üzerine ekliyoruz:
        Vector3 currentRotation = chestBone.localEulerAngles;
        
        // Sadece X eksenini (veya modeline göre doğru ekseni) değiştiriyoruz
        chestBone.localEulerAngles = new Vector3(lookAngle + offsetRotation.x, currentRotation.y, currentRotation.z);
    }
}
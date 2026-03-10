using UnityEngine;
using System.Collections;

public class InteractionSystem : MonoBehaviour
{
    public Transform holdPoint;
    public Camera mainCamera;
    public float pickupRange = 3f;
    public float baseThrowForce = 10f;
    public Animator playerAnimator; 

    public GameObject heldObject; 
    private Rigidbody heldRb;
    private Collider[] heldColliders;
    private HoldableItem currentItemData;

    void Update()
    {
        if (GameInputManager.Instance.InteractTriggered)
        {
            if (heldObject == null) TryPickup();
            else DropObject();
        }
        if (GameInputManager.Instance.ThrowTriggered && heldObject != null) ThrowObject();
    }

    private void TryPickup()
    {
        RaycastHit hit;
        // Raycast atıyoruz
        if (Physics.Raycast(mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)), out hit, pickupRange))
        {
            // 1. ÖNCELİK: Yerdeki eşyayı almak (HoldableItem)
            HoldableItem item = hit.collider.GetComponentInParent<HoldableItem>();
            if (item != null) 
            {
                PickUp(item);
                return; // Eşyayı aldık, fonksiyondan çık. Aşağıdaki koda gitme.
            }

            // 2. ÖNCELİK: Butona basmak (TrapButton)
            // Raycast'in çarptığı objede (veya parentında değil, direkt kendisinde) TrapButton var mı?
            TrapButton button = hit.collider.GetComponent<TrapButton>();
            if (button != null)
            {
                button.PressButton();
                // Butona bastıysak return diyebiliriz veya burada el animasyonu tetikleyebiliriz.
                // playerAnimator.SetTrigger("Push"); // Eğer basma animasyonun varsa
            }
        }
    }

    private void PickUp(HoldableItem item)
    {
        heldObject = item.gameObject;
        currentItemData = item;
        heldRb = heldObject.GetComponent<Rigidbody>();
        heldColliders = heldObject.GetComponentsInChildren<Collider>();

        if (heldRb) { heldRb.isKinematic = true; heldRb.useGravity = false; }
        foreach (var col in heldColliders) col.enabled = false;

        heldObject.transform.SetParent(holdPoint);
        StopAllCoroutines();
        StartCoroutine(SmoothEquipRoutine(currentItemData.holdPositionOffset, currentItemData.holdRotationOffset));
        
        UpdateAnimator(currentItemData.itemType, true);
        currentItemData.OnPickedUp();
    }

    public void DropObject()
    {
        if (heldObject == null) return;
        if (heldRb) { heldRb.isKinematic = false; heldRb.useGravity = true; }
        foreach (var col in heldColliders) col.enabled = true;

        heldObject.transform.SetParent(null);
        UpdateAnimator(currentItemData.itemType, false);
        currentItemData.OnDropped();

        heldObject = null; currentItemData = null; heldRb = null; heldColliders = null;
    }

    private void ThrowObject()
    {
        Rigidbody rbToThrow = heldRb;
        float multiplier = currentItemData.throwForceMultiplier;

        if (heldObject.CompareTag("Ball") && playerAnimator != null)
        {
            playerAnimator.SetTrigger("Shoot");
        }

        DropObject();

        if (rbToThrow != null)
        {
            Vector3 force = mainCamera.transform.forward * (baseThrowForce * multiplier) + mainCamera.transform.up * 2f;
            rbToThrow.AddForce(force, ForceMode.Impulse);
        }
    }

    private void UpdateAnimator(ItemType type, bool isHolding)
    {
        if (playerAnimator == null) return;

        // 1. TEMİZLİK: Önce bütün el durumu bool'larını FALSE yap.
        // Böylece "Drop" edildiğinde (isHolding = false) her şey kapanmış olur ve karakter Idle'a döner.
        playerAnimator.SetBool("isGun", false);
        playerAnimator.SetBool("isMelee", false); 
        playerAnimator.SetBool("isBasketball", false);
        playerAnimator.SetBool("Equip", false);
        // 2. EĞER ELİNE ALIYORSA (EQUIP)
        if (isHolding)
        {
            // İlgili türü TRUE yap
            switch (type)
            {
                case ItemType.Gun:
                    playerAnimator.SetBool("isGun", true);
                    break;
                case ItemType.Melee:
                    playerAnimator.SetBool("isMelee", true);
                    break;
                case ItemType.Basketball:
                    playerAnimator.SetBool("isBasketball", true);
                    break;    
            }

            // Çıkarma animasyonunu tetikle
            playerAnimator.SetBool("Equip", true);
        }
    }

    IEnumerator SmoothEquipRoutine(Vector3 tPos, Vector3 tRot)
    {
        float t = 0f;
        Vector3 sPos = heldObject.transform.localPosition;
        Quaternion sRot = heldObject.transform.localRotation;
        Quaternion eRot = Quaternion.Euler(tRot);
        while (t < 0.2f)
        {
            t += Time.deltaTime;
            heldObject.transform.localPosition = Vector3.Lerp(sPos, tPos, t/0.2f);
            heldObject.transform.localRotation = Quaternion.Slerp(sRot, eRot, t/0.2f);
            yield return null;
        }
    }
}
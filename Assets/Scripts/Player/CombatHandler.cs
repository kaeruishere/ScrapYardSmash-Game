using UnityEngine;

[RequireComponent(typeof(InteractionSystem))]
public class CombatHandler : MonoBehaviour
{
    public Camera fpsCamera;
    
    private InteractionSystem interactionSystem;
    private WeaponBase currentWeapon;

    void Start()
    {
        interactionSystem = GetComponent<InteractionSystem>();
    }

    void Update()
    {
        GameObject heldObj = interactionSystem.heldObject;

        if (heldObj == null)
        {
            currentWeapon = null;
            return;
        }

        // Silah değişti mi?
        if (currentWeapon == null || currentWeapon.gameObject != heldObj)
        {
            currentWeapon = heldObj.GetComponent<WeaponBase>();
            
            // --- GÜNCELLENDİ: Animatörü de gönderiyoruz ---
            if (currentWeapon != null)
            {
                // InteractionSystem zaten playerAnimator'ı tutuyordu, onu kullandık.
                currentWeapon.Initialize(fpsCamera, interactionSystem.playerAnimator);
            }
        }

        if (currentWeapon == null) return;

        if (GameInputManager.Instance.ReloadTriggered) currentWeapon.Reload();
        if (GameInputManager.Instance.FireHeld) currentWeapon.TryAttack();
    }
}
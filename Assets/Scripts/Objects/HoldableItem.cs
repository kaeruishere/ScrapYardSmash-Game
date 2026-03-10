using UnityEngine;

public enum ItemType { Generic, Gun, Melee , Basketball }

public class HoldableItem : MonoBehaviour
{
    public ItemType itemType = ItemType.Generic;
    public Vector3 holdPositionOffset = new Vector3(0.2f, -0.2f, 0.5f);
    public Vector3 holdRotationOffset = Vector3.zero;
    public float throwForceMultiplier = 1f;

    public void OnPickedUp() { }
    public void OnDropped() { }
}
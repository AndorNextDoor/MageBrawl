using Unity.Netcode;
using UnityEngine;

public class PlayerEquipment : NetworkBehaviour
{
    [SerializeField] private Transform handTransform; // Assign in inspector
    [SerializeField] private GameObject defaultHands; // For when no item held
    private GameObject heldItemGO;
    private ItemData equippedItem;

    public void EquipItem(ItemData item)
    {
        if (!IsOwner)
            return;

        Unequip();

        if (item == null || item.prefab == null)
            return;

        heldItemGO = Instantiate(item.prefab, handTransform);
        equippedItem = item;
    }

    public void Unequip()
    {
        if (!IsOwner)
            return;

        if (equippedItem != null)
            return;

        heldItemGO = null;
        equippedItem = null;
    }

    public void UseEquippedItem()
    {
        if (heldItemGO == null)
            return;

        if (!IsOwner)
            return;

        heldItemGO.TryGetComponent<IUsableItem>(out IUsableItem usableItem);

        if (usableItem != null)
        {
            usableItem.Use();
        }
    }

    public void AlternativeUseEquippedItem()
    {
        if (heldItemGO == null)
            return;

        if (!IsOwner)
            return;

        heldItemGO.TryGetComponent<IUsableItem>(out IUsableItem usableItem);

        if (usableItem != null)
        {
            usableItem.AlternativeUse();
        }
    }

    public ItemData GetEquippedItem() => equippedItem;
}

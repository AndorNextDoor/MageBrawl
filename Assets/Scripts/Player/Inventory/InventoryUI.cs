using Unity.Netcode;
using UnityEngine;

public class InventoryUI : NetworkBehaviour
{
    [SerializeField] private int inventorySize = 40;
    public Inventory inventory; // Drag player inventory here
    public Transform slotsParent; // Parent GameObject (GridLayoutGroup)
    public GameObject slotPrefab;

    private SlotUI[] slotUIs;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsOwner)
            return;

        Init();
    }

    public void Init()
    {
        // Spawn slots
        slotUIs = new SlotUI[inventorySize];
        for (int i = 0; i < inventorySize; i++)
        {
            GameObject slotObj = Instantiate(slotPrefab, slotsParent);
            slotUIs[i] = slotObj.GetComponent<SlotUI>();
        }

        UpdateUI();
    }

    public void UpdateUI()
    {
        for (int i = 0; i < inventorySize; i++)
        {
            var slot = inventory.GetSlot(i); 
            slotUIs[i].Set(slot.item, slot.amount);
        }
    }
}

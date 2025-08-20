using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    public Inventory inventory;
    public PlayerEquipment equipment;
    public int hotbarSize = 10;
    private int currentHotbarIndex = -1;

    void Update()
    {
        for (int i = 0; i < hotbarSize; i++)
        {
            if (Input.GetKeyDown((KeyCode)((int)KeyCode.Alpha1 + i)))
            {
                EquipHotbarSlot(i);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            equipment.UseEquippedItem();
        }
    }

    void EquipHotbarSlot(int index)
    {
        if (currentHotbarIndex == index) return;
        currentHotbarIndex = index;

        var slot = inventory.GetSlot(index); // Add a GetSlot(int index) method
        equipment.EquipItem(slot.item);
    }
}

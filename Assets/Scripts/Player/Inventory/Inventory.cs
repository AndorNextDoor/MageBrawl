using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Inventory : NetworkBehaviour
{
    [SerializeField] private ItemsDatabase itemsDatabase;
    [SerializeField] private int inventorySize = 40;
    [SerializeField] private List<InventorySlot> inventory = new List<InventorySlot>();
    private InventoryUI inventoryUI;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsOwner)
            return;

        Init();
    }

    public void Init()
    {
        inventoryUI = GetComponent<InventoryUI>();
        inventory.Clear();
        for (int i = 0; i < inventorySize; i++)
        {
            inventory.Add(new InventorySlot(null, 0)); // An empty slot
        }
        // TO DO:
        // Save items into JSON and load on reconnecting to the game!!!
    }

    public InventorySlot GetSlot(int index) => inventory[index];


    public bool TryAddItem(ItemData item, int amount)
    {
        int spaceAvailable = 0;

        // Count space in existing stacks
        foreach (var slot in inventory)
        {
            if (slot.item != null && slot.item.id == item.id && slot.amount < item.maxStackSize)
            {
                spaceAvailable += (item.maxStackSize - slot.amount);
            }
        }

        // Count empty slots
        foreach (var slot in inventory)
        {
            if (slot.item == null || slot.amount == 0)
            {
                spaceAvailable += item.maxStackSize;
            }
        }

        return spaceAvailable >= amount;
    }


    public void AddItem(ItemData item, int amount)
    {
        int remaining = amount;

        // Try filling existing stacks
        for (int i = 0; i < inventory.Count; i++)
        {
            var slot = inventory[i];
            if(slot.item != null && slot.item.id == item.id && slot.amount < item.maxStackSize)
            {
                int space = item.maxStackSize - slot.amount;
                int toAdd = Mathf.Min(remaining, space);
                slot.amount += toAdd;
                remaining -= toAdd;

                if (remaining == 0)
                {
                    inventoryUI?.UpdateUI();
                    return;
                } 
            }
        }

        // Try adding remaining slots
        for (int i = 0; i < inventory.Count; i++)
        {
            var slot = inventory[i];
            if (slot.item == null || slot.amount == 0)
            {
                int toAdd = Mathf.Min(remaining, item.maxStackSize);
                slot.item = item;
                slot.amount = toAdd;
                remaining -= toAdd;
                inventoryUI?.UpdateUI();

                if (remaining == 0)
                {
                    inventoryUI?.UpdateUI();
                    return;
                }
            }
        }

        if (remaining > 0)
        {
            Debug.Log($"Could not add all items. {remaining} left.");
            inventoryUI?.UpdateUI();
        }
    }

    public bool HaveEnoughItems(ItemData item, int amount)
    {
        return TryRemoveItems(item, amount);
    }

    public bool TryRemoveItems(ItemData item, int amount)
    {
        int neededAmount = amount;

        foreach (var slot in inventory)
        {
            if (slot.item != null && slot.item.id == item.id && slot.amount > 0)
            {
                int toRemove = Mathf.Min(neededAmount, slot.amount);
                neededAmount -= toRemove;
            }
        }

        return neededAmount <= 0;
    }

    public void RemoveItem(ItemData item, int amount)
    {
        int remaining = amount;

        foreach (var slot in inventory)
        {
            if (slot.item != null && slot.item.id == item.id && slot.amount > 0)
            {
                int toRemove = Mathf.Min(remaining, slot.amount);
                slot.amount -= toRemove;
                remaining -= toRemove;

                if (slot.amount == 0)
                    slot.item = null;

                if (remaining == 0)
                {
                    inventoryUI?.UpdateUI();
                    return;
                }
            }
        }

        if (remaining > 0)
        {
            Debug.Log($"Could not remove all items. {remaining} left.");
        }
        inventoryUI?.UpdateUI();
    }


    public void RemoveItemAtSlot(InventorySlot slot, int amount)
    {
        slot.amount -= amount;
        if(slot.amount == 0)
        {
            slot.item = null;
        }
    }

    public bool HaveEnoughItems()
    {
        // TO DO:
        // For later crafting shit
        return true;
    }


}

public class InventorySlot
{
    public ItemData item;
    public int amount;

    public InventorySlot(ItemData item, int amount)
    {
        this.item = item;
        this.amount = amount;
    }
}

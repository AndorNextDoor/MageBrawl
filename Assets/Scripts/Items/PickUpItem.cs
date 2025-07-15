using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public ItemData item;
    public int amount = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerInventoryHolder holder))
        {
            var inventory = holder.inventory;
            if (inventory.TryAddItem(item, amount))
            {
                inventory.AddItem(item, amount);
                Destroy(gameObject); // Picked up
            }
        }
    }
}

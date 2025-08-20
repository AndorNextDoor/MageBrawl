using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Items/New Items Database")]
public class ItemsDatabase : ScriptableObject
{
    public List<ItemData> items;
    
    public ItemData GetItemByID(int id)
    {
        return items[id];
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Items/New item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public string description;
    //
    public GameObject prefab;
    //
    public int id;
    public int maxStackSize;
    //
    public Sprite icon;
    //
    public bool IsUsable;
}

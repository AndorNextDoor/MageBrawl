using UnityEngine;

public class CubeEquippedItem : MonoBehaviour, IUsableItem
{
    public void AlternativeUse()
    {
        Debug.Log("Item used alternatively");
    }

    public void Use()
    {
        Debug.Log("Item used");
    }
}

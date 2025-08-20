using UnityEngine;

public class ShowHideInventory : MonoBehaviour
{
    [SerializeField] private GameObject inventory;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            inventory.SetActive(!inventory.activeInHierarchy);
        }
    }
}

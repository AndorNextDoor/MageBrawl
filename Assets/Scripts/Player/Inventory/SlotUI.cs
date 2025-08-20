using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlotUI : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI amountText;

    public void Set(ItemData item, int amount)
    {
        if (item == null || amount <= 0)
        {
            iconImage.enabled = false;
            amountText.text = "";
        }
        else
        {
            iconImage.enabled = true;
            iconImage.sprite = item.icon; // Ensure your ItemData has a `Sprite icon`
            amountText.text = amount > 1 ? amount.ToString() : "";
        }
    }
}

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpellRadialButton : MonoBehaviour, IPointerEnterHandler
{
    public Image iconImage;
    private SpellData spell;

    public void SetSpell(SpellData newSpell)
    {
        spell = newSpell;
        if (iconImage != null && spell != null)
            iconImage.sprite = spell.icon;
    }

    public SpellData GetSpell()
    {
        return spell;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponentInChildren<Button>().onClick.Invoke();
    }
}

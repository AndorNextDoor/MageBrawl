using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSpellUIManager : MonoBehaviour
{
    [SerializeField] private GameObject spellUIPrefab;
    [SerializeField] private Transform currentSpellsContainer;

    [SerializeField] private PlayerSpellManager spellManager;

    public void OnCurrentSpellsChanged(List<SpellData> spells)
    {
        int childCount = 0;
        foreach (Transform child in currentSpellsContainer)
        {
            child.GetComponentInChildren<Button>().image.sprite = null;
            child.GetComponentInChildren<Button>().image.enabled = false;
            childCount++;
        }

        int i = 0;
        foreach(SpellData spell in spells)
        {
            if (i > childCount)
                break;

            currentSpellsContainer.GetChild(i).GetComponentInChildren<Button>().image.sprite = spell.icon;
            currentSpellsContainer.GetChild(i).GetComponentInChildren<Button>().image.enabled = true;
            i++;
        }
    }
}

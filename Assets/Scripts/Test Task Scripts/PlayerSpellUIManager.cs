using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSpellUIManager : MonoBehaviour
{
    [SerializeField] private Transform spellsUIContainer;
    [SerializeField] private GameObject spellUIPrefab;

    [SerializeField] private PlayerSpellManager spellManager;


    private void LoadAllSpellsInMenu()
    {
        List<SpellData> spellsList = spellManager.GetAllSpells();

        foreach (SpellData spell in spellsList)
        {
            GameObject newSpell = Instantiate(spellUIPrefab, spellsUIContainer);

            SpellData capturedSpell = spell;

            Button newSpellButton = newSpell.GetComponent<Button>();
            newSpellButton.image.sprite = spell.icon;
            newSpellButton.onClick.AddListener(() => spellManager.AddNewSpellToQueue(capturedSpell)); 
        }      
    }
}

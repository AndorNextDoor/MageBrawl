using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSpellUIManager : MonoBehaviour
{
    [SerializeField] private Transform spellsUIContainer;
    [SerializeField] private GameObject spellUIPrefab;
    [SerializeField] private Transform currentSpellsContainer;

    [SerializeField] private PlayerSpellManager spellManager;

    private void Awake()
    {
        LoadAllSpellsInMenu();
    }

    private void LoadAllSpellsInMenu()
    {
        List<SpellData> spellsList = spellManager.GetAllSpells();

        foreach (SpellData spell in spellsList)
        {
            if (spell.element == ElementType.None)
                continue;

            GameObject newSpell = Instantiate(spellUIPrefab, spellsUIContainer);

            SpellData capturedSpell = spell;

            Button newSpellButton = newSpell.GetComponentInChildren<Button>();
            newSpellButton.image.sprite = spell.icon;
            newSpellButton.onClick.AddListener(() => spellManager.AddNewSpellToQueue(capturedSpell)); 
        }      
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            spellsUIContainer.gameObject.SetActive(!spellsUIContainer.gameObject.activeSelf);
            
            if (spellsUIContainer.gameObject.activeSelf)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

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

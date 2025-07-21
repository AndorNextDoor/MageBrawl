using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PlayerSpellManager : MonoBehaviour
{
    [SerializeField] List<SpellData> spellsList = new List<SpellData>();

    [SerializeField] private int maxSpellSlots = 2;
    [SerializeField] List< SpellData> currentSpellsQueue = new List<SpellData>(); // For multicasting

    [SerializeField] private PlayerSpellUIManager spellUIManager;


    public SpellData GetSpellByName(string spellName)
    {
        return spellsList.Find((x) => x.spellName == spellName);
    }

    public List<SpellData> GetAllSpells()
    {
        return spellsList;
    }

    public void AddNewSpellToQueue(SpellData _newSpell)
    {
        if (currentSpellsQueue.Count >= maxSpellSlots)
        {
            currentSpellsQueue.RemoveAt(0);
        }

        currentSpellsQueue.Add(_newSpell);

        spellUIManager.OnCurrentSpellsChanged(currentSpellsQueue);
    }


    public void ClearSpellQueue()
    {
        currentSpellsQueue.Clear();

        spellUIManager.OnCurrentSpellsChanged(currentSpellsQueue);
    }

    public SpellData GetCurrentSpell()
    {
        if (currentSpellsQueue.Count == 0)  return null;
        

        if(currentSpellsQueue.Count > 1)
        {
            SpellData combinedSpell = GetCombinedSpell();

            if(combinedSpell == null)
            {
                currentSpellsQueue.Clear();
            }

            return combinedSpell;
        }
        else
        {
            return currentSpellsQueue[0];
        }
    }

    public SpellData GetCombinedSpell()
    {
        List<ElementType> elements = new List<ElementType>();

        foreach (var spell in currentSpellsQueue)
        {
            elements.Add(spell.element);
        }

        // Sort to make order irrelevant
        elements.Sort();

        // Find spell that matches these elements
        foreach (var spell in spellsList)
        {
            if (spell.combination == null || spell.combination.Count == 0)
                continue;

            var combo = new List<ElementType>(spell.combination);
            combo.Sort();

            if (combo.Count == elements.Count && combo.SequenceEqual(elements))
            {
                return spell;
            }
        }

        return null;
    }

}

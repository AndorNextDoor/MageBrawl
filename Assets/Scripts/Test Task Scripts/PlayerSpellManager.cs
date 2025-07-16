using UnityEngine;
using System.Collections.Generic;

public class PlayerSpellManager : MonoBehaviour
{
    [SerializeField] List<SpellData> spellsList = new List<SpellData>();

    [SerializeField] private int maxSpellSlots = 2;
    [SerializeField] List<  SpellData> currentSpellsQueue = new List<SpellData>(); // For multicasting


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
    }


    public void ClearSpellQueue()
    {
        currentSpellsQueue.Clear();
    }

    public SpellData GetCurrentSpell()
    {
        if (currentSpellsQueue.Count == 0)  return null;
        

        if(currentSpellsQueue.Count > 1)
        {
            return GetCombinedSpell();
        }
        else
        {
            return currentSpellsQueue[0];
        }
    }

    public SpellData GetCombinedSpell()
    {
        string combinedSpellString = "";

        foreach(SpellData spell in currentSpellsQueue)
        {
            combinedSpellString += spell.spellName;
        }

        return GetSpellByName(combinedSpellString);
    }
}

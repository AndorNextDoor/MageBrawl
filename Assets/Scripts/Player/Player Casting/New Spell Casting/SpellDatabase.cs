using System.Collections.Generic;
using UnityEngine;

public class SpellDatabase : MonoBehaviour
{
    public static SpellDatabase Instance;

    [SerializeField] private SpellSO[] spells;

    private Dictionary<string, SpellSO> spellDict = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Build dictionary for fast lookup
        foreach (var spell in spells)
        {
            if (spell != null && !spellDict.ContainsKey(spell.name))
            {
                spellDict.Add(spell.name, spell);
            }
            else
            {
                Debug.LogWarning($"Duplicate or null spell found in SpellDatabase: {spell?.name}");
            }
        }
    }

    public SpellSO GetSpellByName(string spellName)
    {
        if (spellDict.TryGetValue(spellName, out var spell))
        {
            return spell;
        }

        Debug.LogWarning($"Spell '{spellName}' not found in database.");
        return null;
    }

    public SpellSO GetSpellByIndex(int index)
    {
        if (index > spells.Length)
            return null;

        return spells[index];
    }

    public int SpellCount()
    {
        return spells.Length;
    }


    public SpellSO[] GetAllSpells() => spells;
}

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spells", menuName = "New Spell")]
public class SpellData : ScriptableObject
{
    public string spellName;
    public ElementType element;
    public float damage;
    public float force;

    public bool IsChanneling;
    public float maxChannelingTime;

    public GameObject spellPrefab;
    public List<ElementType> combination;
    public Sprite icon;
}

public enum ElementType
{
    None,
    Fire,
    Wind,
    Water,
    Earth,
}


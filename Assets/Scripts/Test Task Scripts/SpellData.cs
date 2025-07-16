using UnityEngine;

[CreateAssetMenu(fileName = "Spells", menuName = "New Spell")]
public class SpellData : ScriptableObject
{
    public string spellName;
    public float damage;
    public float force;

    public bool IsChanneling;
    public float maxChannelingTime;

    public Sprite icon;
}

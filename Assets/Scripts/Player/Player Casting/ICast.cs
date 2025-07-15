using UnityEngine;

public interface ICast
{
    public void Cast();
    public void SetStats(SpellSO spellSO, Transform shootTransform, PlayerSpellUI playerSpellUI, int spellID);
}


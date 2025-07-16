using UnityEngine;

public interface ISpell
{
    void Cast(Transform caster);
    void Stop(); // For channeled spells
}
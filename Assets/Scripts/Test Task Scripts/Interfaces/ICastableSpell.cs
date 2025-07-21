using UnityEngine;

public interface ICastableSpell
{
    void StartCasting(Transform caster, SpellData data);

    void StopCasting(Transform caster, SpellData data);

}

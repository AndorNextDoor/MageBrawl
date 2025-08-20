using UnityEngine;

[CreateAssetMenu (fileName = "Spells/New Spell", menuName = "Spells/New Spell")]
public class SpellSO : ScriptableObject
{
    public Sprite icon;
    public GameObject projectilePrefab;
    public GameObject castingPrefab; //Attach to player to be able to cast
    public GameObject dummyPrefab;
    public PlayerSpellCastingAnimator.SpellCastAnimationType animationType;
    public float castTime = 1f;

    public float cooldown;
    public float knockback;
    public float duration;
}

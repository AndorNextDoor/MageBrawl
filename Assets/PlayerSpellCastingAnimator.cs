using Unity.Netcode;
using UnityEngine;

public class PlayerSpellCastingAnimator : NetworkBehaviour
{
    [SerializeField] private Animator animator;

    public enum SpellCastAnimationType
    {
        Attack1, 
        Attack2, 
        Attack3, 
        Attack4
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        PlayerSpellCastingNew spellCasting = GetComponent<PlayerSpellCastingNew>();
        spellCasting.OnSpellCasted += SpellCasting_OnSpellCasted;
    }

    private void SpellCasting_OnSpellCasted(SpellCastAnimationType animationType)
    {
        animator.SetTrigger(animationType.ToString());
    }


    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        PlayerSpellCastingNew spellCasting = GetComponent<PlayerSpellCastingNew>();
        spellCasting.OnSpellCasted -= SpellCasting_OnSpellCasted;
    }

    
}

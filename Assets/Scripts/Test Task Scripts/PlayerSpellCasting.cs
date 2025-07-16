using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpellCastingTestManager : MonoBehaviour
{
    [SerializeField] private InputActionReference attackAction;
    [SerializeField] private PlayerSpellManager spellManager;

    private SpellData currentSpell;

    private void Start()
    {
        attackAction.action.started += StartCasting;
        attackAction.action.canceled += StopCasting;

    }

    private void StartCasting(InputAction.CallbackContext obj)
    {
        currentSpell = spellManager.GetCurrentSpell();
     
        if(currentSpell == null)
        {
            Debug.Log("No spells in the queue!");
        }

        Debug.Log("Casted spell: " + currentSpell.spellName);
        // Start casting
    }
 
    private void StopCasting(InputAction.CallbackContext obj)
    {
        Debug.Log("Stopped casting: " + currentSpell.spellName);
        currentSpell = null;
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpellCastingTestManager : MonoBehaviour
{
    [SerializeField] private InputActionReference attackAction;
    [SerializeField] private PlayerSpellManager spellManager;
    [SerializeField] private Transform spellCastPoint;

    private SpellData currentSpell;
    private GameObject activeSpellInstance;
    private ICastableSpell activeCastable;

    private void Start()
    {
        attackAction.action.started += StartCasting;
        attackAction.action.canceled += StopCasting;
    }

    private void StartCasting(InputAction.CallbackContext obj)
    {
        if (Cursor.lockState != CursorLockMode.Locked)
            return;

        currentSpell = spellManager.GetCurrentSpell();
        if (currentSpell == null)
        {
            Debug.Log("No spells in the queue!");
            return;
        }

        spellManager.ClearSpellQueue();

        if (currentSpell.spellPrefab != null)
        {
            activeSpellInstance = Instantiate(currentSpell.spellPrefab, spellCastPoint.position, spellCastPoint.rotation);
            activeCastable = activeSpellInstance.GetComponent<ICastableSpell>();

            if (activeCastable != null)
            {
                activeCastable.StartCasting(spellCastPoint, currentSpell);
            }
            else
            {
                Debug.LogWarning("Spell prefab does not implement ICastableSpell");
            }
        }

        Debug.Log("Started casting: " + currentSpell.spellName);
    }

    private void StopCasting(InputAction.CallbackContext obj)
    {
        if (Cursor.lockState != CursorLockMode.Locked)
            return;

        if (currentSpell == null)
        {
            Debug.Log("No spell to stop");
            return;
        }

        if (activeCastable != null)
        {
            activeCastable.StopCasting(transform, currentSpell);
        }

        Debug.Log("Stopped casting: " + currentSpell.spellName);

        currentSpell = null;
        activeSpellInstance = null;
        activeCastable = null;
    }
}

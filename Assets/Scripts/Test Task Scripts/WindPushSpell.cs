using UnityEngine;

public class WindPushSpell : MonoBehaviour, ICastableSpell
{
    public float radius = 5f;
    public float pushForce = 20f;
    public LayerMask pushableLayers;

    public void StartCasting(Transform caster, SpellData data)
    {
        Vector3 origin = caster.position + caster.forward * 1f; // a bit in front of the player

        Collider[] hitColliders = Physics.OverlapSphere(origin, radius, pushableLayers);
        foreach (var hit in hitColliders)
        {
            Rigidbody rb = hit.attachedRigidbody;
            if (rb != null)
            {
                Vector3 forceDir = (hit.transform.position - caster.position).normalized;
                rb.AddForce(forceDir * data.force, ForceMode.Impulse);
            }
        }

        // Optional: VFX
        Debug.DrawRay(origin, caster.forward * radius, Color.cyan, 2f);
        Destroy(gameObject, 2f); // Clean up effect if needed
    }

    public void StopCasting(Transform caster, SpellData data)
    {

    }
}

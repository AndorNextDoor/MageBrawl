using UnityEngine;

public class FireballSpellTest : MonoBehaviour, ICastableSpell
{
    public float speed = 40f;
    private float damage;
    private float knockbackForce;

    public LayerMask hitLayers;

    private Rigidbody rb;
    private bool hasBeenCast = false;
    private Transform caster;

    public void StartCasting(Transform _caster, SpellData data)
    {
        caster = _caster;
        damage = data.damage;
        knockbackForce = data.force;

        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(caster.forward * speed, ForceMode.Force);
        }

        hasBeenCast = true;
    }

    public void StopCasting(Transform _caster, SpellData data)
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasBeenCast || caster == null)
            return;

        if (((1 << other.gameObject.layer) & hitLayers) == 0)
            return;

        
        IDamageableTest damageable = other.GetComponent<IDamageableTest>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
        }

        
        Rigidbody targetRb = other.attachedRigidbody;
        if (targetRb != null)
        {
            Vector3 knockDir = (other.transform.position - caster.position).normalized;
            targetRb.AddForce(knockDir * knockbackForce, ForceMode.Impulse);
        }

        // ADD EFFECT OF EXPLOSION
        Destroy(gameObject);
    }
}

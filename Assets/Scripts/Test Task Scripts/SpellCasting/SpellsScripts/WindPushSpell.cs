using UnityEngine;

public class WindPushSpell : MonoBehaviour, ICastableSpell
{
    public float radius = 5f;
    public float pushForce = 20f;
    public LayerMask pushableLayers;
    private Transform caster;
    private SpellData data;

    private bool isCasting = false;

    private float castTimer = 0;

    [SerializeField] private GameObject particleEffect;

    private GameObject currentParticle;

    public void StartCasting(Transform _caster, SpellData _data)
    {
        caster = _caster;
        data = _data;
        isCasting = true;
    }

    public void StopCasting(Transform caster, SpellData data)
    {
        isCasting = false;
        Destroy(currentParticle);
        Destroy(gameObject);
    }

    private void Update()
    {
        if (!isCasting || caster == null) return;

        castTimer += Time.deltaTime;
        if (castTimer >= data.maxChannelingTime)
        {
            StopCasting(caster, data);
        }

        Vector3 origin = caster.position + caster.forward * 1f;
        Collider[] hitColliders = Physics.OverlapSphere(origin, radius, pushableLayers);
        foreach (var hit in hitColliders)
        {
            Rigidbody rb = hit.attachedRigidbody;
            if (rb != null)
            {
                Vector3 forceDir = (hit.transform.position - caster.position).normalized;
                rb.AddForce(forceDir * data.force * Time.deltaTime, ForceMode.VelocityChange);
            }
        }

        // Optional debug
        Debug.DrawRay(origin, caster.forward * radius, Color.cyan);
    }
}

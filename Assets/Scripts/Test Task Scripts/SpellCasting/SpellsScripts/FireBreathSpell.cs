using UnityEngine;

public class FireBreathSpell : MonoBehaviour, ICastableSpell
{
    public float radius = 5f;
    public float damagePerSecond = 20f;
    public LayerMask damageableLayers;

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
        castTimer = 0;

        currentParticle = Instantiate(particleEffect, caster);
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
        if(castTimer >= data.maxChannelingTime)
        {
            StopCasting(caster, data);
        }
        Collider[] hitColliders = Physics.OverlapSphere(caster.position, radius, damageableLayers);

        foreach (var hit in hitColliders)
        {
            IDamageableTest damageable = hit.GetComponent<IDamageableTest>();
            if (damageable != null)
            {
                damageable.TakeDamage(damagePerSecond * Time.deltaTime);
            }
        }
    }
}

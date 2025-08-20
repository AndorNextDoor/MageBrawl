using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Fireball : NetworkBehaviour, ICast
{
    private int spellID;
    private Transform _shootTransform;
    private float cooldownTimer = Mathf.Infinity;

    private SpellSO spellStats;

    public event Action<int, float> OnSpellCasted;

    private GameObject castingPreviewInstance;

    public void Cast()
    {
        if (!IsOwner)
            return;

        if (cooldownTimer > 0)
        {
            Debug.Log("Spell on cooldown");
            return;
        }

        cooldownTimer = spellStats.cooldown;
        OnSpellCasted?.Invoke(spellID, spellStats.cooldown);

        ShowCastingPreviewServerRpc();

        StartCoroutine(DelayedFireballSpawn());

        // Real authoritative cast
        CastFireballServerRpc();
    }

    private IEnumerator DelayedFireballSpawn()
    {
        yield return new WaitForSeconds(spellStats.castTime);

        // Remove casting preview
        DestroyCastingPreviewServerRpc();

        // Spawn actual fireball
        CastFireballServerRpc();
    }

    [ServerRpc]
    private void ShowCastingPreviewServerRpc()
    {
        if (!IsServer)
            return;

        GameObject preview = Instantiate(spellStats.dummyPrefab, _shootTransform.position, _shootTransform.rotation);
        preview.GetComponent<NetworkObject>().Spawn(true); // spawn to all
        castingPreviewInstance = preview;
    }

    [ServerRpc]
    private void DestroyCastingPreviewServerRpc()
    {
        if (!IsServer)
            return;

        if (castingPreviewInstance != null && castingPreviewInstance.TryGetComponent(out NetworkObject netObj))
        {
            netObj.Despawn();
            Destroy(castingPreviewInstance);
            castingPreviewInstance = null;
        }
    }

    public void CastFireballServer()
    {
        GameObject _proj = Instantiate(spellStats.projectilePrefab, _shootTransform.position, _shootTransform.rotation);
        var netObj = _proj.GetComponent<NetworkObject>();
        netObj.Spawn();

        var knock = _proj.GetComponent<SpellKnockback>();
        knock.SetKnockback(spellStats.knockback);
        knock.SetOwner(OwnerClientId); // ✅ Pass owner here
    }


    [ServerRpc]
    public void CastFireballServerRpc()
    {
        if (!IsServer)
            return;

        CastFireballServer();
    }

    private void Update()
    {
        if (!IsOwner)
            return;

        if (spellStats == null)
            return;

        cooldownTimer -= Time.deltaTime;
    }


    public void SetStats(SpellSO _spellSO, Transform shootTransform, PlayerSpellUI playerSpellUI, int _spellID)
    {
        spellStats = _spellSO;

        if (_spellSO == null)
            return;

        cooldownTimer = _spellSO.cooldown;
        _shootTransform = shootTransform;

        OnSpellCasted += playerSpellUI.OnSpellCasted;

        spellID = _spellID;
    }

    private void SpawnDummyProjectile()
    {
        GameObject dummy = Instantiate(spellStats.projectilePrefab, _shootTransform.position, _shootTransform.rotation);
        Destroy(dummy, 1.5f); // Auto-destroy after a while
        var netObj = dummy.GetComponent<NetworkObject>();
        if (netObj) Destroy(netObj); // Optional: remove networking if prefab includes it
    }
}

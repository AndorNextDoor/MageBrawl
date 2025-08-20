using System.Collections;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class FireballSpell : NetworkBehaviour, ISpellBehaviour
{
    private SpellSO spellData;
    private Transform shootOrigin;
    private ulong casterClientId;
    private float cooldownTimer = Mathf.Infinity;
    private GameObject castingPreviewInstance;

    public void Init(SpellSO data, Transform shootOrigin, ulong casterClientId)
    {
        this.spellData = data;
        this.shootOrigin = shootOrigin;
        this.casterClientId = casterClientId;
    }

    public void OnCast()
    {
        if (!IsOwner) return;

        //// Real spawn
        //FireballServerRpc();

        ShowCastingPreviewServerRpc();
        StartCoroutine(DelayedFireballSpawn());
    }
    private IEnumerator DelayedFireballSpawn()
    {
        yield return new WaitForSeconds(spellData.castTime);

        // Remove casting preview
        DestroyCastingPreviewServerRpc();

        // Spawn actual fireball
        FireballServerRpc();
    }


    [ServerRpc]
    private void ShowCastingPreviewServerRpc()
    {
        if (!IsServer)
            return;

        GameObject preview = Instantiate(spellData.dummyPrefab, shootOrigin);

        SpellGrowBigger spellGrow;
        preview.TryGetComponent<SpellGrowBigger>(out spellGrow);

        if(spellGrow != null)
        {
            spellGrow.SetGrowDuration(spellData.castTime * 0.9f);
        }

        FollowClientObject following;
        if (preview.TryGetComponent(out following))
        {
            following.TargetClientId = NetworkObject.OwnerClientId;
        }


        preview.GetComponent<NetworkObject>().Spawn(true); // spawn to all
        castingPreviewInstance = preview;
    }
    [ServerRpc]
    void FireballServerRpc()
    {
        GameObject proj = Instantiate(spellData.projectilePrefab, shootOrigin.position, shootOrigin.rotation);
        var netObj = proj.GetComponent<NetworkObject>();
        netObj.Spawn();

        var knock = proj.GetComponent<SpellKnockback>();
        knock.SetKnockback(spellData.knockback);
        knock.SetOwner(casterClientId);
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
}

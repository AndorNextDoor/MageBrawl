using Unity.Netcode;
using UnityEngine;

public class SpellKnockback : NetworkBehaviour
{
    public float knockBackForce { get; set; }

    private ulong ownerClientId;

    public void SetKnockback(float value) => knockBackForce = value;
    public void SetOwner(ulong clientId) => ownerClientId = clientId;

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer)
            return;

        // Don't hit the owner
        if (other.TryGetComponent<NetworkObject>(out var netObj))
        {
            if (netObj.OwnerClientId == ownerClientId)
                return;
        }

        if (other.TryGetComponent<PlayerKnockBack>(out var otherPlayer))
        {
            otherPlayer.TakeKnockBack(knockBackForce, transform.forward);
        }

        if (other.TryGetComponent<HealthComponent>(out var health))
        {
            health.RegisterHitSource(ownerClientId);
        }


        GetComponent<NetworkObject>().Despawn(true);
    }
}

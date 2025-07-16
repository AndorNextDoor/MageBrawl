using Unity.Netcode;
using UnityEngine;

public class PlayerKnockBack : NetworkBehaviour
{
    [SerializeField] private float knockbackDefence = 1f;

    public void TakeKnockBack(float knockbackForce, Vector3 direction)
    {
        ApplyKnockbackClientRpc(knockbackForce, direction);
    }

    [ClientRpc]
    private void ApplyKnockbackClientRpc(float knockbackForce, Vector3 direction)
    {
        if (!IsOwner) return;

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(direction * knockbackForce, ForceMode.Impulse);
    }

}

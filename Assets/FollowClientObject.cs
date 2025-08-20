using System;
using Unity.Netcode;
using UnityEngine;

public class FollowClientObject : NetworkBehaviour
{
    private Transform transformToFollow;
    public float followSmoothSpeed = 10f;

    public ulong TargetClientId;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // Only run on client who owns the target we're supposed to follow
        if (IsClient && IsSpawned && IsOwner == true)
        {
            SetFollowTargetClientRpc(TargetClientId);
        }
    }

    [ClientRpc]
    private void SetFollowTargetClientRpc(ulong playerID)
    {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(playerID, out var client))
        {
            transformToFollow = client.PlayerObject.GetComponent<PlayerShootPosition>().GetShootTranform();
        }
        else
        {
            Debug.LogWarning($"Client with ID {playerID} not found.");
        }
    }




    private void LateUpdate()
    {
        if (transformToFollow == null) return;

        transform.position = Vector3.Lerp(transform.position, transformToFollow.position, Time.deltaTime * followSmoothSpeed);
    }

}

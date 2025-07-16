using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerInfo : NetworkBehaviour
{
    public NetworkVariable<FixedString64Bytes> PlayerName = new NetworkVariable<FixedString64Bytes>(
        "", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        if (IsOwner && IsClient)
        {
            // Send the name to server on spawn
            SubmitPlayerNameServerRpc(PlayerPrefs.GetString("PlayerName"));
        }
    }

    [ServerRpc]
    private void SubmitPlayerNameServerRpc(string name)
    {
        PlayerName.Value = name;
    }
}

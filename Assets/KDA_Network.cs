using System;
using Unity.Netcode;
using UnityEngine;

public class KDA_Network : NetworkBehaviour
{
    public NetworkVariable<float> Kills = new NetworkVariable<float>(
    readPerm: NetworkVariableReadPermission.Everyone,
    writePerm: NetworkVariableWritePermission.Server);

    public NetworkVariable<float> Deaths = new NetworkVariable<float>(
    readPerm: NetworkVariableReadPermission.Everyone,
    writePerm: NetworkVariableWritePermission.Server);

    public NetworkVariable<float> Assists = new NetworkVariable<float>(
    readPerm: NetworkVariableReadPermission.Everyone,
    writePerm: NetworkVariableWritePermission.Server);

    public event Action OnKDAOpen;
    public event Action OnKDAClosed;

    private void Update()
    {
        if (!IsOwner)
            return;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            OnKDAOpen?.Invoke();
        }

        if (Input.GetKeyUp(KeyCode.Tab))
        {
            OnKDAClosed?.Invoke();
        }

    }

    [ServerRpc]
    public void AddKillServerRpc()
    {
        Kills.Value++;
    }

    [ServerRpc]
    public void AddDeathServerRpc()
    {
        Deaths.Value++;
    }

    [ServerRpc]
    public void AddAssistServerRpc()
    {
        Assists.Value++;
    }


}

using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayersConnecting : NetworkBehaviour
{
    public static PlayersConnecting Instance;

    //Players connecting 
    private int currentPlayerCount = 0;
    private HashSet<ulong> readyPlayers = new();

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

            //currentPlayerCount++;
            //if (currentPlayerCount >= GameSettingsCarrier.Instance.ExpectedPlayers)
            //    StartGame();
        }

        if (Instance == null)
            Instance = this;
    }


    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"Client connected: {clientId}");
        currentPlayerCount++;

        if (GameSettingsCarrier.Instance == null)
        {
            StartGame();
            return;
        }

        if (currentPlayerCount >= GameSettingsCarrier.Instance.ExpectedPlayers)
            StartGame();
    }


    private void StartGame()
    {
        RoundManager.Instance.SetTimerOnPlayersConnected();
    }





     // Maybe redo that later

    [ServerRpc(RequireOwnership = false)]
    public void PlayerReadyServerRpc(ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;

        if (!readyPlayers.Contains(clientId))
        {
            readyPlayers.Add(clientId);
            Debug.Log($"Player {clientId} is ready. {readyPlayers.Count}/{GameSettingsCarrier.Instance.ExpectedPlayers}");

            if (readyPlayers.Count >= GameSettingsCarrier.Instance.ExpectedPlayers)
            {
                StartGame();
            }
        }
    }

    [ClientRpc]
    private void StartRoundClientRpc(int round)
    {
        Debug.Log($"[Client] Round {round} started!");
        // Trigger round logic on each client (UI, timers, enabling spells etc.)
    }

}

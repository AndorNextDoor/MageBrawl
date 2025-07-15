using System;
using System.Collections.Generic;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using Unity.Netcode;
using UnityEngine;
using static RoundManager;

public class RoundManager : NetworkBehaviour
{
    public static RoundManager Instance;

    public enum RoundPhases
    {
        Shop,
        Combat
    } 

    public NetworkVariable<bool> playersCanPlay = new(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<RoundPhases> currentRoundPhase = new(writePerm: NetworkVariableWritePermission.Server);

    public event Action <RoundPhases> currentRoundPhaseChanged;

    private bool lastRoundWasStoreRound = false;

    [SerializeField] private Transform centerOfMap;
    [SerializeField] private float spawnRadius;

    private int currentRound = -1; // So first fight round is index 0
    [SerializeField] private float[] roundTimers; // Duration of combat rounds
    [SerializeField] private float buyTimer = 8f;

    private NetworkVariable<float> currentRoundTimer = new(writePerm: NetworkVariableWritePermission.Server);

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        currentRoundTimer.Value = 60;
    }

    private void Update()
    {
        if (!IsServer || currentRoundTimer.Value <= 0)
            return;

        currentRoundTimer.Value -= Time.deltaTime;

        if (currentRoundTimer.Value <= 0)
        {
            if (lastRoundWasStoreRound)
                StartCombatRound();
            else
                StartBuyRound();
        }
    }

    public void SetTimerOnPlayersConnected()
    {
        if (!IsServer)
            return;

        currentRoundTimer.Value = 5f;
    }

    private void StartBuyRound()
    {
        Debug.Log("Buy round started");
        lastRoundWasStoreRound = true;
        playersCanPlay.Value = false;
        currentRoundPhase.Value = RoundPhases.Shop;
        currentRoundPhaseChanged?.Invoke(currentRoundPhase.Value);

        currentRoundTimer.Value = buyTimer;

        FreezeAllPlayers();
        ResetPlayersPositions();
        ResetPlayersHealth();
        UnFreezeAllPlayers();

        StartBuyRoundClientRpc();
        // Optionally notify clients
        // RoundTypeClientRpc(false); // false = store round
    }

    [ClientRpc]
    private void StartBuyRoundClientRpc()
    {
        var localPlayer = NetworkManager.Singleton.LocalClient?.PlayerObject;
        if (localPlayer == null)
        {
            Debug.LogWarning("Local player not found!");
            return;
        }

        var spellChoose = localPlayer.GetComponentInChildren<SpellChoose>(true);
        if (spellChoose != null)
        {
            spellChoose.ShowRandomSkills(3);
            Debug.Log("Buy menu opened for local player.");
        }
        else
        {
            Debug.LogWarning("SpellChoose component not found on local player.");
        }
    }


    private void StartCombatRound()
    {
        Debug.Log("Combat round started");
        lastRoundWasStoreRound = false;
        playersCanPlay.Value = true;
        currentRoundPhase.Value = RoundPhases.Combat;
        currentRoundPhaseChanged?.Invoke(currentRoundPhase.Value);

        currentRound++;
        if (currentRound >= roundTimers.Length)
        {
            Debug.Log("All rounds completed");
            EndGame();
            return;
        }

        currentRoundTimer.Value = roundTimers[currentRound];

        // RoundTypeClientRpc(true); // true = combat round
    }

    private void EndGame()
    {
        playersCanPlay.Value = false;
        // Show score screen etc
    }

    public void FreezeAllPlayers()
    {
        if (!IsServer) return;

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            var player = client.PlayerObject;
            if (player == null) continue;

            var rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.constraints = RigidbodyConstraints.FreezeAll;
            }
        }
    }

    public void UnFreezeAllPlayers()
    {
        if (!IsServer) return;

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            var player = client.PlayerObject;
            if (player == null) continue;

            var rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.constraints = RigidbodyConstraints.FreezeRotation;
                // Optional: rb.constraints = RigidbodyConstraints.FreezeRotationY;
            }
        }
    }

    public void ResetPlayersPositions()
    {
        if (!IsServer) return;

        Vector3 center = centerOfMap.position;
        int playerCount = NetworkManager.Singleton.ConnectedClients.Count;
        int index = 0;

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            var player = client.PlayerObject;
            if (player == null) continue;

            float angle = (360f / playerCount) * index;
            float radian = angle * Mathf.Deg2Rad;
            Vector3 newPosition = center + new Vector3(Mathf.Cos(radian), 0, Mathf.Sin(radian)) * spawnRadius;
            newPosition.y = 5;

            if (client.ClientId == NetworkManager.Singleton.LocalClientId)
            {
                ResetPositionHost(newPosition, center);
            }

        var rpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new List<ulong> { client.ClientId }
            }
        };

        ResetPositionsClientRpc(newPosition, center, rpcParams);


        ResetPositionsClientRpc(newPosition, center, rpcParams);
            index++;
        }
    }

    private void ResetPositionHost(Vector3 newPos, Vector3 center)
    {
        var player = NetworkManager.Singleton.LocalClient?.PlayerObject;
        if (player == null) return;

        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        player.GetComponent<ClientNetworkTransform>()?.Teleport(newPos, Quaternion.LookRotation(center - newPos), player.transform.localScale);

        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    [ClientRpc]
    private void ResetPositionsClientRpc(Vector3 newPos, Vector3 center, ClientRpcParams rpcParams = default)
    {
        var localPlayer = NetworkManager.Singleton.LocalClient?.PlayerObject;
        if (localPlayer == null) return;

        var rb = localPlayer.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        var clientTransform = localPlayer.GetComponent<ClientNetworkTransform>();
        if (clientTransform != null)
        {
            clientTransform.Teleport(newPos, Quaternion.LookRotation(center - newPos), localPlayer.transform.localScale);
        }

        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }
    public void ResetPlayersHealth()
        {
            foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            {
                if (client.PlayerObject.TryGetComponent<HealthComponent>(out var health))
                {
                    health.CurrentHealth.Value = health.MaxHealth;
                }
            }
        }


        public float GetTimer() => currentRoundTimer.Value;
        public RoundPhases GetCurrentPhase() => currentRoundPhase.Value;
    }




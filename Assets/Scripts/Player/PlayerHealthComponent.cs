using System.Globalization;
using System;
using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class HealthComponent : NetworkBehaviour, IDamagable
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private PlayerHealthUI healthUI;

    public event Action<float, float> OnHealthChanged;

    public float MaxHealth => maxHealth;

    public NetworkVariable<float> CurrentHealth = new NetworkVariable<float>(
        readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Server);

    public bool IsAlive => CurrentHealth.Value > 0;

    private struct DamageSourceEntry
    {
        public ulong attackerId;
        public float time;
    }

    private List<DamageSourceEntry> recentSources = new();
    private float assistTimeWindow = 5f;


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer) 
        {
            CurrentHealth.Value = maxHealth;
        }

        // Hook up UI event even for non-owner
        CurrentHealth.OnValueChanged += (oldVal, newVal) =>
        {
            OnHealthChanged?.Invoke(newVal, maxHealth);
        };
    }

    public void RegisterHitSource(ulong attackerId)
    {
        if (!IsServer) return;

        float time = Time.time;
        recentSources.Add(new DamageSourceEntry { attackerId = attackerId, time = time });

        // Clean up old sources
        recentSources.RemoveAll(e => time - e.time > assistTimeWindow);
    }

    public void TakeDamage(float damage)
    {
        if (IsServer)
        {
            ApplyDamage(damage);
        }
        else
        {
            ApplyDamageServerRpc(damage);
        }
    }

    private void ApplyDamage(float amount)
    {
        if (!IsAlive || !RoundManager.Instance.playersCanPlay.Value)
            return;

        CurrentHealth.Value = Mathf.Clamp(CurrentHealth.Value - amount, 0, maxHealth);

        OnHealthChanged?.Invoke(CurrentHealth.Value, maxHealth);

        if (CurrentHealth.Value <= 0)
        {
            Die();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ApplyDamageServerRpc(float amount)
    {
        ApplyDamage(amount);
    }



    public void TakeHealing(float heal)
    {
        if (!IsServer || !IsAlive) return;
        if (!RoundManager.Instance.playersCanPlay.Value)
            return;

        CurrentHealth.Value = Mathf.Clamp(CurrentHealth.Value + heal, 0, maxHealth);
    }

    public void Die()
    {
        if (!IsServer) return;

        //NetworkObject.Despawn();

        if (IsServer)
        {
            HandleAttributedDeath();
        }
        else
        {
            HandleAttributedDeathServerRpc();
        }

    }

    private void HandleAttributedDeath()
    {
        if (!IsServer) return;

        float currentTime = Time.time;

        var valid = recentSources.Where(e => currentTime - e.time <= assistTimeWindow).ToList();

        ulong? killerId = valid.Count > 0 ? valid.Last().attackerId : null;
        var assistIds = valid.Select(e => e.attackerId)
                             .Where(id => id != killerId)
                             .Distinct();

        // Add Death to self
        GetComponent<KDA_Network>()?.AddDeathServerRpc();

        // Kill
        if (killerId.HasValue && NetworkManager.Singleton.ConnectedClients.TryGetValue(killerId.Value, out var killerClient))
        {
            if (killerId == OwnerClientId)
                return;
            killerClient.PlayerObject.GetComponent<KDA_Network>()?.AddKillServerRpc();
        }

        // Assists
        foreach (var aid in assistIds)
        {
            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(aid, out var client))
            {
                client.PlayerObject.GetComponent<KDA_Network>()?.AddAssistServerRpc();
            }
        }

        recentSources.Clear();
    }

    [ServerRpc]
    private void HandleAttributedDeathServerRpc()
    {
        HandleAttributedDeath();
    }

}

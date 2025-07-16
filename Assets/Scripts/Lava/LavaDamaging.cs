using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LavaDamageZone : NetworkBehaviour
{
    [SerializeField] private float damagePerSecond = 10f;
    private Dictionary<HealthComponent, float> playersInLava = new();

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        if (other.transform.TryGetComponent(out HealthComponent health))
        {
            if (!playersInLava.ContainsKey(health))
            {
                playersInLava.Add(health, 0f);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsServer) return;

        if (other.transform.TryGetComponent(out HealthComponent health))
        {
            if (playersInLava.ContainsKey(health))
            {
                playersInLava.Remove(health);
            }
        }
    }

    private void Update()
    {
        if (!IsServer || playersInLava.Count == 0)
            return;

        List<HealthComponent> toRemove = new();
        List<HealthComponent> toDamage = new();

        // Copy the keys to safely iterate
        foreach (var health in new List<HealthComponent>(playersInLava.Keys))
        {
            if (health == null || !health.IsAlive)
            {
                toRemove.Add(health);
                continue;
            }

            playersInLava[health] += Time.deltaTime;

            if (playersInLava[health] >= 1f)
            {
                toDamage.Add(health);
                playersInLava[health] = 0f;
            }
        }

        // Apply damage outside of the iteration
        foreach (var h in toDamage)
        {
            h.TakeDamage(damagePerSecond);
        }

        foreach (var h in toRemove)
        {
            playersInLava.Remove(h);
        }
    }

}

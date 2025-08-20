using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : NetworkBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private HealthComponent playerHealth;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsOwner)
        {
            healthSlider.gameObject.SetActive(false);
            return;
        }

        playerHealth.OnHealthChanged += SetHealthSlider;
    }


    public void SetHealthSlider(float currentValue, float maxValue)
    {
        if (!IsOwner)
            return;

        healthSlider.value = currentValue;
        healthSlider.maxValue = maxValue;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        playerHealth.OnHealthChanged -= SetHealthSlider;
    }
}

using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameAndHealthUI : NetworkBehaviour
{
    [SerializeField] private GameObject holderUI;
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private Slider healthSlider;

    [SerializeField] private HealthComponent playerHealth;

    private PlayerNameNetwork playerNameNetwork;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            holderUI.SetActive(false); // Hide UI for yourself
        }

        playerNameNetwork = GetComponent<PlayerNameNetwork>();
        if (playerNameNetwork != null)
        {
            playerName.text = playerNameNetwork.PlayerName.Value.ToString();

            playerNameNetwork.PlayerName.OnValueChanged += (oldVal, newVal) =>
            {
                playerName.text = newVal.ToString();
            };
        }

        playerHealth.OnHealthChanged += UpdateHealthUI;
    }

    private void UpdateHealthUI(float current, float max)
    {
        healthSlider.maxValue = max;
        healthSlider.value = current;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        playerHealth.OnHealthChanged -= UpdateHealthUI;
    }
}

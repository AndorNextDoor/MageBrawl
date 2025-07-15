using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerGoldUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goldText;

    private PlayerGold playerGold;

    private void Start()
    {
        // Delay finding player until spawn is complete
        StartCoroutine(FindLocalPlayer());
    }

    private IEnumerator FindLocalPlayer()
    {
        while (NetworkManager.Singleton.LocalClient == null || NetworkManager.Singleton.LocalClient.PlayerObject == null)
            yield return null;

        playerGold = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerGold>();
        if (playerGold == null)
        {
            Debug.LogError("PlayerGold not found on local player!");
            yield break;
        }

        // Initial UI update
        UpdateGoldUI(playerGold.CurrentGold.Value);

        // Subscribe to gold changes
        playerGold.CurrentGold.OnValueChanged += OnGoldChanged;
    }

    private void OnDestroy()
    {
        if (playerGold != null)
            playerGold.CurrentGold.OnValueChanged -= OnGoldChanged;
    }

    private void OnGoldChanged(int oldVal, int newVal)
    {
        UpdateGoldUI(newVal);
    }

    private void UpdateGoldUI(int gold)
    {
        goldText.text = $"Gold: {gold}" + "$";
    }
}

using TMPro;
using Unity.Netcode;
using UnityEngine;

public class KDA_UI : NetworkBehaviour
{
    [SerializeField] private KDA_Network KDA_network;

    [SerializeField] private GameObject KDA_ToDisable;
    [SerializeField] private GameObject KDA_Prefab;
    [SerializeField] private GameObject KDA_Holder;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsOwner)
        {
            KDA_ToDisable.SetActive(false);
            return;
        }

        KDA_ToDisable.SetActive(false);
        
        KDA_network.OnKDAOpen   += OnUpdateKDA;
        KDA_network.OnKDAClosed += OnKDAClosedUI;
    }

    public void OnUpdateKDA()
    {
        if (!IsOwner)
            return;

        foreach (Transform child in KDA_Holder.transform)
            Destroy(child.gameObject);

        foreach (var player in FindObjectsByType<KDA_Network>(FindObjectsSortMode.None))
        {
            var playerInfo = player.GetComponent<PlayerInfo>();
            if (playerInfo == null) continue;

            GameObject kdaEntry = Instantiate(KDA_Prefab, KDA_Holder.transform);

            TextMeshProUGUI[] texts = kdaEntry.GetComponentsInChildren<TextMeshProUGUI>();

            if (texts.Length >= 4)
            {
                texts[0].text = playerInfo.PlayerName.Value.ToString(); // Player name
                texts[1].text = player.Kills.Value.ToString("0");       // Kills
                texts[2].text = player.Deaths.Value.ToString("0");      // Deaths
                texts[3].text = player.Assists.Value.ToString("0");     // Assists
            }
        }

        KDA_ToDisable.SetActive(true);
    }

    public void OnKDAClosedUI()
    {
        KDA_ToDisable.SetActive(false);
    }



    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn(); // was wrong before

        if (!IsOwner)
            return;

        KDA_network.OnKDAOpen   -= OnUpdateKDA;
        KDA_network.OnKDAClosed -= OnKDAClosedUI;
    }

}

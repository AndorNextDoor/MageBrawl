using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovementStateUI : NetworkBehaviour
{
    [SerializeField] private PlayerMovementStateController playerMovementState;
    [SerializeField] private TextMeshProUGUI currentStateText;
    [SerializeField] private GameObject playerMovementUI;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsOwner)
        {
            playerMovementUI.SetActive(false);
        }
    }

    private void Update()
    {
        if (!IsOwner)
            return;

        currentStateText.text = playerMovementState.GetCurrentState().ToString();
    }
}

using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerGold : NetworkBehaviour
{
    [SerializeField] private int StartingGold = 100;

    public NetworkVariable<int> CurrentGold = new(
        readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Server);

    private bool IsInCombat = false;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            CurrentGold.Value = StartingGold;

            if (RoundManager.Instance != null)
                RoundManager.Instance.currentRoundPhaseChanged += OnRoundPhaseChanged;
        }

        if (IsOwner)
        {
            StartCoroutine(GetGoldWhileInCombat());
        }
    }


    public override void OnDestroy()
    {
        if (IsServer && RoundManager.Instance != null)
        {
            RoundManager.Instance.currentRoundPhaseChanged -= OnRoundPhaseChanged;
        }
    }

    public void TrySpendGoldRequest(int amount)
    {
        TrySpendGoldServerRpc(amount);
    }

    [ServerRpc]
    public void TrySpendGoldServerRpc(int amount)
    {
        if (amount > CurrentGold.Value)
            return;

        CurrentGold.Value -= amount;
    }

    private void SpendGold(int amount)
    {
        CurrentGold.Value -= amount;
    }

    IEnumerator GetGoldWhileInCombat()
    {
        Debug.Log("Gold coroutine started on " + OwnerClientId);

        while (true)
        {
            if (!IsOwner)
                yield break;

            if (IsInCombat)
            {
                Debug.Log("Gaining gold...");
                GetGoldServerRpc(2);
            }

            yield return new WaitForSeconds(1f);
        }
    }


    [ServerRpc]
    private void GetGoldServerRpc(int amount)
    {
        CurrentGold.Value += amount;
    }

    private void OnRoundPhaseChanged(RoundManager.RoundPhases roundPhase)
    {
        ChangePhaseClientRpc(roundPhase == RoundManager.RoundPhases.Combat);
    }

    [ClientRpc]
    private void ChangePhaseClientRpc(bool _IsInCombat)
    {
        IsInCombat = _IsInCombat;
    }
}

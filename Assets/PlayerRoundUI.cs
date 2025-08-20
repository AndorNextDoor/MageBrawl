using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerRoundUI : NetworkBehaviour
{
    [SerializeField] private GameObject roundHolder; 
    [SerializeField] private TextMeshProUGUI roundTimer;
    [SerializeField] private TextMeshProUGUI roundPhase;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsOwner)
        {
            roundHolder.SetActive(false);
            return;
        }

        RoundManager.Instance.currentRoundPhaseChanged += OnRoundPhaseChanged;
    }


    private void Update()
    {
        if (!RoundManager.Instance || !IsOwner) return;

        roundPhase.text = RoundManager.Instance.GetCurrentPhase().ToString();
        float time = RoundManager.Instance.GetTimer();
        roundTimer.text = FormatTime(time);
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        return $"{minutes:00}:{seconds:00}";
    }

    private void OnRoundPhaseChanged(RoundManager.RoundPhases _roundPhase)
    {
        roundPhase.text = _roundPhase.ToString();
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        RoundManager.Instance.currentRoundPhaseChanged -= OnRoundPhaseChanged;
    }

}

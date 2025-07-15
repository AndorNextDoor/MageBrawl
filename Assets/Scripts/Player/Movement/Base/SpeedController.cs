using Unity.Netcode;
using UnityEngine;

public class SpeedController : NetworkBehaviour
{
    private PlayerMovementStateController _stateMachine;
    private MovementConfig _config;
    private PlayerGlobalStats _globalStats;

    private float currentSpeed;
    public float GetCurrentSpeed => currentSpeed + (currentSpeed * _globalStats.speedMultiplier);

    [SerializeField] private float acceleration  = 10f;
    [SerializeField] private float deceleration = 12f;

    private float targetSpeed;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        _stateMachine = GetComponent<PlayerMovementStateController>();
        _config = GetComponent<MovementConfig>();
        _globalStats = GetComponent<PlayerGlobalStats>();


        base.OnNetworkSpawn();
    }

    private void Update()
    {
        if (!IsOwner) return;

        UpdateTargetSpeed();
        SmoothSpeed();
    }

    private void UpdateTargetSpeed()
    {
        switch (_stateMachine.GetCurrentState())
        {
            case MovementState.Idle:
                targetSpeed = 0f;
                break;
            case MovementState.Moving:
                targetSpeed = _config.WalkSpeed;
                break;
            case MovementState.Sprinting:
                targetSpeed = _config.SprintSpeed;
                break;
            case MovementState.Crouching:
                targetSpeed = _config.WalkSpeed * 0.5f; // or use crouch speed
                break;
            case MovementState.Air:
                targetSpeed = _config.WalkSpeed * 0.8f;
                break;
        }
    }

    private void SmoothSpeed()
    {
        float rate = (targetSpeed > currentSpeed) ? acceleration : deceleration;
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, rate * Time.deltaTime);
    }
}

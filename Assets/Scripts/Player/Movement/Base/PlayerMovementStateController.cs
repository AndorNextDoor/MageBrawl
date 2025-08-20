using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovementStateController : NetworkBehaviour
{
    private MovementState _currentState;
    private PlayerInputReader _inputReader;
    private GroundCheck _groundCheck;

    public event Action<MovementState> OnStateChanged;
    public event Action<MovementState> OnStateExit;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        _inputReader = GetComponent<PlayerInputReader>();
        _groundCheck = GetComponent<GroundCheck>();
    }

    private void FixedUpdate()
    {
        if (!IsOwner || !RoundManager.Instance.playersCanPlay.Value)
            return;

        if (!_groundCheck.IsGrounded)
        {
            ChangeState(MovementState.Air);
        }
        else if (_inputReader.IsJumping() && _currentState != MovementState.Crouching)
        {
            ChangeState(MovementState.Jumping);
            Debug.Log("Jumped");
        }
        else if (_inputReader.IsCrouching())
        {
            ChangeState(MovementState.Crouching);
        }
        else if (_inputReader.IsSprinting())
        {
            ChangeState(MovementState.Sprinting);
        }
        else if (_inputReader.IsMoving())
        {
            ChangeState(MovementState.Moving);
        }
        else
        {
            ChangeState(MovementState.Idle);
        }
    }

    #region CanChangeStateBools
    private bool CanJump()
    {
        return _inputReader.IsJumping();
    }
    #endregion

    public void ChangeState(MovementState state)
    {
        if (_currentState == state) return;

        OnStateExit?.Invoke(_currentState);
        _currentState = state;
        OnStateChanged?.Invoke(state);
    }

    public MovementState GetCurrentState() => _currentState;
    public bool CanMove()
    {
        return _inputReader.IsMoving();
    }
}


public enum MovementState
{
    Idle,
    Moving,
    Sprinting,
    Jumping,
    Air,
    Crouching
}
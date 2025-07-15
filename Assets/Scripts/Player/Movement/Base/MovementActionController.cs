using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace PlayerCharacter
{
    public class MovementActionController : NetworkBehaviour
    {
        //References
        [SerializeField] private MovementConfig movementConfig;
        private readonly Dictionary<MovementState, IPlayerMovementAction> _actions = new();
        private PlayerMovementStateController _movementState;

        #region Initialization
        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;

            // ADD ACTIONS
            _actions[MovementState.Moving] = new PlayerWalk();
          //  _actions[MovementState.Sprinting] = new PlayerSprint();
           // _actions[MovementState.Crouching] = new PlayerCrouch();
           // _actions[MovementState.Jumping] = new PlayerJump();


            // ENABLE ACTIONS
            EnableAllActions();

            // REFERENCES
            _movementState = GetComponent<PlayerMovementStateController>();

            // EVENTS
            _movementState.OnStateChanged += PlayAction;
            _movementState.OnStateExit += StopAction;

            base.OnNetworkSpawn();
        }

        private void EnableAllActions()
        {
            foreach (MovementState actionType in _actions.Keys)
            {
                _actions[actionType].Enable(gameObject);
            }
        }
        #endregion

        #region MovementActions

        private void FixedUpdate()
        {
            if (!IsOwner)
                return;

            if (_movementState.CanMove())
            {
                PlayAction(MovementState.Moving);
            }
        }

        private void PlayAction(MovementState state)
        {
            if (!RoundManager.Instance.playersCanPlay.Value)
                return;
            if (_actions.TryGetValue(state, out var action))
            {
                action.PlayAction();
               // Debug.Log("Action played: " + action.ToString());
            }
        }

        private void StopAction(MovementState state)
        {
            if(_actions.TryGetValue(state, out var action))
            {
                action.StopAction();
            }
        }
        #endregion


        public override void OnNetworkDespawn()
        {
            _movementState.OnStateChanged -= PlayAction;
            _movementState.OnStateExit -= StopAction;

            base.OnNetworkDespawn();
        }

    }
}



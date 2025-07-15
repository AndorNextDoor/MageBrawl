using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReader : NetworkBehaviour, IPlayerInputSource
{
    // BOOLS
    private bool _isMoving;
    private bool _isSpinting;
    private bool _isJumping;
    private bool _isCrouching;

    // Player Input
    private PlayerInput playerInput;
    [Header("Player Input Actions")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference sprintAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference crouchAction;


    // Bools to return
    private Vector2 _InputMoveVector = Vector2.zero;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
            return;

        // References
        playerInput = GetComponent<PlayerInput>();

        // Player Inputs
        // MOVE
        moveAction.action.Enable();
        moveAction.action.performed += OnMove;
        moveAction.action.canceled += OnMoveEnd;

        // SPRINT
        sprintAction.action.Enable();
        sprintAction.action.started += OnSprintPressed;
        sprintAction.action.canceled += OnSprintReleased;

        // JUMP
        jumpAction.action.Enable();
        jumpAction.action.started += OnJumpPressed;
        jumpAction.action.canceled += OnJumpReleased;


        // CROUCH
        crouchAction.action.Enable();
        crouchAction.action.started += OnCrouchPressed;
        crouchAction.action.canceled += OnCrouchReleased;

        base.OnNetworkSpawn();
    }

    #region On Buttons Pressed Actions

    #region IS_MOVING

    private void OnMove(InputAction.CallbackContext context)
    {
        _InputMoveVector = context.ReadValue<Vector2>();
        _isMoving = _InputMoveVector.magnitude > 0.1f;
    }

    private void OnMoveEnd(InputAction.CallbackContext context)
    {
        _InputMoveVector = Vector2.zero;
        _isMoving = false;
    }

    #endregion

    #region IsSprinting
    private void OnSprintPressed(InputAction.CallbackContext context)
    {
        _isSpinting = true;
    }

    private void OnSprintReleased(InputAction.CallbackContext context)
    {
        _isSpinting = false;
    }
    #endregion

    #region IsJumping
    private void OnJumpPressed(InputAction.CallbackContext context)
    {
        _isJumping = true;
    }   
    
    private void OnJumpReleased(InputAction.CallbackContext context)
    {
        _isJumping = false;
    }
    #endregion

    #region IsCrouching
    private void OnCrouchPressed(InputAction.CallbackContext context)
    {
        _isCrouching = true;
    }

    private void OnCrouchReleased(InputAction.CallbackContext context)
    {
        _isCrouching = false;
    }
    #endregion

    #endregion

    #region Return Inputs


    public Vector2 GetMoveInput()
    {
        return _InputMoveVector;
    }

    public bool IsMoving()
    {
        return _isMoving;
    }

    public bool IsSprinting()
    {
        return _isSpinting;
    }

    public bool IsCrouching()
    {
        return _isCrouching;
    }

    public bool IsJumping()
    {
        return _isJumping;
    }


    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;

        moveAction.action.performed -= OnMove;
        moveAction.action.canceled -= OnMoveEnd;
        sprintAction.action.started -= OnSprintPressed;
        sprintAction.action.canceled -= OnSprintReleased;
        jumpAction.action.started -= OnJumpPressed;
        jumpAction.action.canceled -= OnJumpReleased;
        crouchAction.action.started -= OnCrouchPressed;
        crouchAction.action.canceled -= OnCrouchReleased;

        base.OnNetworkDespawn();
    }

    #endregion
}

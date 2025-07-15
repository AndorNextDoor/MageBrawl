using UnityEngine;

public class PlayerWalk : IPlayerMovementAction
{
    private IMovementStates _playerState;
    private IPlayerInputSource _input;

    private GameObject _player;
    private Rigidbody _rb;

    private Transform _orientation;

    private SpeedController _speedController;
    private PlayerGlobalStats _globalStats;

    private float acceleration = 10f;
    private float maxSpeed;
    private float drag = .1f; // Adjust for slipperiness (lower = more slippery)

    public void Enable(GameObject player)
    {
        _player = player;
        _playerState = player.GetComponent<IMovementStates>();

        _rb = player.GetComponent<Rigidbody>();
        _input = player.GetComponent<IPlayerInputSource>();

        _speedController = player.GetComponent<SpeedController>();
        _orientation = player.GetComponent<CameraController>().GetOrientation();

        _globalStats = player.GetComponent<PlayerGlobalStats>();


    }
    public void PlayAction()
    {
        if (_rb == null || _input == null || _orientation == null) return;

        maxSpeed = _speedController.GetCurrentSpeed;

        Vector2 moveInput = _input.GetMoveInput();
        Vector3 inputDir = new Vector3(moveInput.x, 0f, moveInput.y);
        if (inputDir.sqrMagnitude > 1f)
            inputDir.Normalize();

        // Movement relative to camera (orientation)
        Vector3 moveDir = _orientation.right * inputDir.x + _orientation.forward * inputDir.z;
        moveDir.y = 0;

        // Apply acceleration
        _rb.linearVelocity += moveDir * acceleration * Time.fixedDeltaTime;

        // Clamp to max speed
        Vector3 horizontalVel = new Vector3(_rb.linearVelocity.x, 0, _rb.linearVelocity.z);
        if (horizontalVel.magnitude > maxSpeed)
        {
            Vector3 clampedVel = horizontalVel.normalized * maxSpeed;
            _rb.linearVelocity = new Vector3(clampedVel.x, _rb.linearVelocity.y, clampedVel.z);
        }

        // Apply drag manually for slippery deceleration
        Vector3 dragVec = new Vector3(_rb.linearVelocity.x, 0, _rb.linearVelocity.z) * drag * Time.fixedDeltaTime;
        _rb.linearVelocity -= new Vector3(dragVec.x, 0, dragVec.z);
    }



    public void StopAction()
    {
        // Nothing to stop for now 
    }
}
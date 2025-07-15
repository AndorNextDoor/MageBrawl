using UnityEngine;

public class PlayerJump : IPlayerMovementAction
{
    PlayerMovementStateController _stateController;

    private Rigidbody _rb;

    public void Enable(GameObject player)
    {
        _rb = player.GetComponent<Rigidbody>();
        _stateController = player.GetComponent<PlayerMovementStateController>();
    }

    public void PlayAction()
    {
        Jump();
    }

    public void StopAction()
    {
        // Nothing to do for now
    }

    private void Jump()
    {
        _rb.AddForce(Vector3.up * 10 , ForceMode.Impulse);
        _stateController.ChangeState(MovementState.Air);
    }
}
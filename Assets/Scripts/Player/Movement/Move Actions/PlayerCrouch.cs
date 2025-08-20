using UnityEngine;

public class PlayerCrouch : IPlayerMovementAction
{
    private IMovementStates _playerState;

    private GameObject _player;

    //private Transform _playerTransform;
    //private Vector3 _originalTransform;
    //private Vector3 _crouchedTransform;

    public void Enable(GameObject player)
    {
        _player = player;
        _playerState = player.GetComponent<IMovementStates>();

/*        _playerTransform = player.transform;
        _originalTransform = player.transform.localScale;
        _crouchedTransform = new Vector3(_originalTransform.x, _originalTransform.y / 2f, _originalTransform.z);*/
    }

    public void PlayAction()
    {
        Crouch();
    }

    public void StopAction()
    {
        UnCrouch();
    }

    private void Crouch()
    {
        //_playerTransform.localScale = _crouchedTransform;
    }

    private void UnCrouch()
    {
        //_playerTransform.localScale = _originalTransform;
    }
}
using UnityEngine;

public class PlayerSprint : IPlayerMovementAction
{
    private IMovementStates _playerState;

    private GameObject _player;
    public void Enable(GameObject player)
    {
        _player = player;
        _playerState = player.GetComponent<IMovementStates>();
    }

    public void PlayAction()
    {
        StartSprinting();
    }

    public void StopAction()
    {
        StopSprinting();
    }

    private void StartSprinting()
    {
        Debug.Log("Start sprinting");
    }

    private void StopSprinting()
    {
        Debug.Log("Stop sprinting");
    }
}
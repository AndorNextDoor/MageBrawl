using UnityEngine;

public interface IPlayerMovementAction
{
    public void PlayAction();

    public void StopAction();

    public void Enable(GameObject player);
}
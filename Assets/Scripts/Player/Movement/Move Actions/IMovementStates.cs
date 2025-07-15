using UnityEngine;

public interface IMovementStates
{
    public void TrySettingNewState(MovementState newState);

    public void LeaveState(MovementState state);
}

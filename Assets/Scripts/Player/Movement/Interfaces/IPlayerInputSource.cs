using System;
using UnityEngine;

public interface IPlayerInputSource
{
    //MOVE
    public bool IsMoving();
    // SPRINT
    public bool IsSprinting();
    // CROUCH
    public bool IsCrouching();
    // JUMP
    public bool IsJumping();
    // MOVE
    Vector2 GetMoveInput();
}

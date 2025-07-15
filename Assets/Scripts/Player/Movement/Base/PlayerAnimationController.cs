using Unity.Netcode;
using UnityEngine;

public class PlayerAnimationController : NetworkBehaviour, IAnimationStates
{
    [SerializeField] private Animator animator;
    private PlayerMovementStateController _movementState;

    public override void OnNetworkSpawn()
    {
        _movementState = GetComponent<PlayerMovementStateController>();
        _movementState.OnStateChanged += EnterAnimationState;

        base.OnNetworkSpawn();
    }

    private void EnterAnimationState(MovementState movementState)
    {
        EnterAnimation(movementState, animator);
    }

    public void EnterAnimation(MovementState movementState, Animator animator)
    {
        animator.StopPlayback();

        switch (movementState)
        {
            case MovementState.Idle:
                animator.SetBool("Walk", false);
                break;
            case MovementState.Moving:
                animator.SetBool("Walk", true);
                break;
            //case MovementState.Sprinting:
            //    animator.Play("Sprint");
            //    break;
            //case MovementState.Jumping:
            //    animator.Play("Jump");
                //break;
            case MovementState.Air:
                animator.Play("Airborne");
                break;
            //case MovementState.Crouching:
            //    animator.Play("Crouch");
                //break;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            animator.Play("Silly Dancing");
        }
    }
}

using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float checkDistance = 0.3f;
    [SerializeField] private Transform groundCheckPoint;

    public bool IsGrounded { get; private set; }

    private void Update()
    {
        IsGrounded = Physics.Raycast(groundCheckPoint.position, Vector3.down, checkDistance, groundMask);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheckPoint == null) return;
        Gizmos.color = IsGrounded ? Color.green : Color.red;
        Gizmos.DrawLine(groundCheckPoint.position, groundCheckPoint.position + Vector3.down * checkDistance);
    }
}

using UnityEngine;

public class SpawnVisualizer : MonoBehaviour
{
    [SerializeField] private int playerCount = 4;
    [SerializeField] private float radius = 10f;
    [SerializeField] private Vector3 center = Vector3.zero;
    [SerializeField] private Color gizmoColor = Color.green;

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;

        for (int i = 0; i < playerCount; i++)
        {
            float angle = (360f / playerCount) * i;
            float radian = angle * Mathf.Deg2Rad;

            Vector3 pos = center + new Vector3(Mathf.Cos(radian), 0f, Mathf.Sin(radian)) * radius;

            // Draw a sphere at each spawn point
            Gizmos.DrawSphere(pos, 0.5f);

            // Draw a line to center
            Gizmos.DrawLine(center, pos);
        }

        // Optional: draw the full circle
        DrawCircle(center, radius, 64);
    }

    private void DrawCircle(Vector3 center, float radius, int segments)
    {
        float angleStep = 360f / segments;
        Vector3 prevPoint = center + new Vector3(Mathf.Cos(0), 0, Mathf.Sin(0)) * radius;

        for (int i = 1; i <= segments; i++)
        {
            float rad = Mathf.Deg2Rad * angleStep * i;
            Vector3 nextPoint = center + new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * radius;
            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }
    }
}

using UnityEngine;

[ExecuteAlways]
public class GizmoCircle : MonoBehaviour
{
    [Header("Configurações do Círculo")]
    public float radius = 1f;
    public Color color = Color.yellow;
    public int segments = 32;

    private void OnDrawGizmos()
    {
        DrawCircle();
    }

    private void OnDrawGizmosSelected()
    {
        // Reforço opcional para exibir o círculo também quando selecionado
        DrawCircle();
    }

    private void DrawCircle()
    {
        Gizmos.color = color;

        Vector3 prevPoint = transform.position + transform.right * radius;
        float angleStep = 360f / segments;

        for (int i = 1; i <= segments; i++)
        {
            float angle = angleStep * i * Mathf.Deg2Rad;
            Vector3 nextPoint = transform.position +
                new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }
    }
}

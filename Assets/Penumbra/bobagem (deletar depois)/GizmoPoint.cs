using UnityEngine;

[ExecuteAlways]
public class GizmoPoint : MonoBehaviour
{
    [Header("Configurações do Gizmo")]
    public Color color = Color.yellow;
    public float size = 0.15f;

    private void OnDrawGizmos()
    {
        DrawPoint();
    }

    private void OnDrawGizmosSelected()
    {
        DrawPoint(); // reforça visibilidade ao selecionar
    }

    private void DrawPoint()
    {
        Gizmos.color = color;
        Gizmos.DrawSphere(transform.position, size);
    }
}

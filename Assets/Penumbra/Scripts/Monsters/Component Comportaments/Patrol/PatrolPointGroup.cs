using UnityEngine;

public class PatrolPointGroup : MonoBehaviour
{
    [Tooltip("Pontos de patrulha dentro deste grupo")]
    public Transform[] patrolPoints;

    [Header("Debug")]
    public Color gizmoColor = Color.cyan;
    public float gizmoRadius = 0.3f;

    private void OnDrawGizmos()
    {
        if (patrolPoints == null) return;

        Gizmos.color = gizmoColor;

        foreach (var p in patrolPoints)
        {
            if (p == null) continue;
            Gizmos.DrawSphere(p.position, gizmoRadius);
        }

        // desenha ligações
        for (int i = 0; i < patrolPoints.Length - 1; i++)
        {
            if (patrolPoints[i] != null && patrolPoints[i + 1] != null)
            {
                Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[i + 1].position);
            }
        }
    }
}

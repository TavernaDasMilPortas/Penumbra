using UnityEngine;

public class FacingSystem : MonoBehaviour
{
    [Header("Configura��o de vis�o")]
    public float viewAngle = 90f; // �ngulo de vis�o (em graus)
    public float viewDistance = 10f; // dist�ncia m�xima de vis�o

    [Header("Debug")]
    public bool showGizmos = true;

    // Retorna true se um alvo estiver dentro do campo de vis�o
    public bool IsLookingAt(Vector3 targetPosition)
    {
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;

        // checa dist�ncia
        if (Vector3.Distance(transform.position, targetPosition) > viewDistance)
            return false;

        // checa �ngulo
        float angle = Vector3.Angle(transform.forward, directionToTarget);
        return angle < viewAngle * 0.5f;
    }

    // �til para intera��es (pega um collider mais pr�ximo que esteja na frente)
    public Collider GetClosestFacingTarget(LayerMask layerMask)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, viewDistance, layerMask);
        Collider closest = null;
        float closestDist = Mathf.Infinity;

        foreach (var hit in hits)
        {
            if (IsLookingAt(hit.transform.position))
            {
                float dist = Vector3.Distance(transform.position, hit.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = hit;
                }
            }
        }

        return closest;
    }

    private void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewDistance);

        // desenha cone de vis�o
        Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle / 2, 0) * transform.forward;
        Vector3 rightBoundary = Quaternion.Euler(0, viewAngle / 2, 0) * transform.forward;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * viewDistance);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * viewDistance);
    }
}

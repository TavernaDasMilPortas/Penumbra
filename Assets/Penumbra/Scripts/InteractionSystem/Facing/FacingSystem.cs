using UnityEngine;

public class FacingSystem : MonoBehaviour
{
    [Header("Configuração de visão")]
    public float viewAngle = 90f; // ângulo de visão (em graus)
    public float viewDistance = 10f; // distância máxima de visão

    [Header("Debug")]
    public bool showGizmos = true;

    // Retorna true se um alvo estiver dentro do campo de visão
    public bool IsLookingAt(Vector3 targetPosition)
    {
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;

        // checa distância
        if (Vector3.Distance(transform.position, targetPosition) > viewDistance)
            return false;

        // checa ângulo
        float angle = Vector3.Angle(transform.forward, directionToTarget);
        return angle < viewAngle * 0.5f;
    }

    // Útil para interações (pega um collider mais próximo que esteja na frente)
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

        // desenha cone de visão
        Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle / 2, 0) * transform.forward;
        Vector3 rightBoundary = Quaternion.Euler(0, viewAngle / 2, 0) * transform.forward;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * viewDistance);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * viewDistance);
    }
}

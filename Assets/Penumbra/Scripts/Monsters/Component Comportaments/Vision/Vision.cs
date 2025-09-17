using UnityEngine;

public class Vision : MonoBehaviour
{
    [Header("Configura��o de Vis�o")]
    public float viewRadius = 10f;      // Dist�ncia m�xima
    [Range(0, 360)]
    public float viewAngle = 90f;       // �ngulo de vis�o (ex: 90 graus = vis�o em cone)

    [Tooltip("Camadas que podem bloquear a vis�o (ex: paredes)")]
    public LayerMask obstacleMask;

    [Tooltip("Camada que o player pertence")]
    public LayerMask targetMask;

    private Transform target;

    private void Start()
    {
        // Procura o player automaticamente (tag)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
        }
    }

    public bool CanSeePlayer()
    {
        if (target == null) return false;

        Vector3 dirToTarget = (target.position - transform.position).normalized;
        float distToTarget = Vector3.Distance(transform.position, target.position);

        // Est� dentro do raio de vis�o?
        if (distToTarget <= viewRadius)
        {
            // Est� dentro do �ngulo de vis�o?
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2f)
            {
                // Faz um raycast para checar se n�o tem parede no meio
                if (!Physics.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        // Gizmos para visualizar o cone no editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Vector3 leftBoundary = DirFromAngle(-viewAngle / 2, false);
        Vector3 rightBoundary = DirFromAngle(viewAngle / 2, false);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * viewRadius);
    }

    private Vector3 DirFromAngle(float angleDegrees, bool global)
    {
        if (!global)
        {
            angleDegrees += transform.eulerAngles.y;
        }
        float rad = angleDegrees * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(rad), 0, Mathf.Cos(rad));
    }

}

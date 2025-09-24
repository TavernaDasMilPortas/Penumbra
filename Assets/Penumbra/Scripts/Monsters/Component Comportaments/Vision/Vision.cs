using UnityEngine;
using UnityEngine.AI;

public class Vision : MonoBehaviour
{
    [Header("Configura��o de Vis�o")]
    public float viewRadius = 10f;
    [Range(0, 360)]
    public float viewAngle = 90f;

    [Tooltip("Camadas que podem bloquear a vis�o (ex: paredes)")]
    public LayerMask obstacleMask;

    [Tooltip("Camada que o player pertence")]
    public LayerMask targetMask;

    private Transform target;
    private NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

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

        if (distToTarget <= viewRadius)
        {
            // Frente real do inimigo (se parado usa transform.forward)
            Vector3 viewDir = GetViewDirection();

            if (Vector3.Angle(viewDir, dirToTarget) < viewAngle / 2f)
            {
                if (!Physics.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private Vector3 GetViewDirection()
    {
        if (agent != null && agent.velocity.sqrMagnitude > 0.01f)
        {
            return agent.velocity.normalized; // frente = dire��o que est� andando
        }
        else
        {
            return transform.forward; // frente padr�o
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Vector3 viewDir = Application.isPlaying ? GetViewDirection() : transform.forward;

        Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle / 2f, 0) * viewDir;
        Vector3 rightBoundary = Quaternion.Euler(0, viewAngle / 2f, 0) * viewDir;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * viewRadius);
    }
}

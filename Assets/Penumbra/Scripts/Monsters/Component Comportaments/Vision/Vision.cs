using UnityEngine;
using UnityEngine.AI;

public class Vision : MonoBehaviour
{
    [Header("Configuração de Visão")]
    public float viewRadius = 10f;
    [Range(0, 360)]
    public float viewAngle = 90f;

    [Tooltip("Camadas que podem bloquear a visão (ex: paredes)")]
    public LayerMask obstacleMask;

    [Tooltip("Camada que o player pertence")]   
    public LayerMask targetMask;

    public Transform target;
    private NavMeshAgent agent;

    private bool lastSeePlayer = false; // controle para evitar spam de logs

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
            Debug.Log($"{name}: Player encontrado → {player.name}");
        }
        else
        {
            Debug.LogWarning($"{name}: Nenhum objeto com tag 'Player' encontrado!");
        }
    }

    public bool CanSeePlayer()
    {
        if (target == null)
        {
            Debug.LogWarning($"{name}: Target está null, não pode ver o player.");
            return false;
        }

        Vector3 dirToTarget = (target.position - transform.position).normalized;
        float distToTarget = Vector3.Distance(transform.position, target.position);

        if (distToTarget <= viewRadius)
        {
            Vector3 viewDir = GetViewDirection();

            if (Vector3.Angle(viewDir, dirToTarget) < viewAngle / 2f)
            {
                if (!Physics.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask))
                {
                    if (!lastSeePlayer)
                    {
                        Debug.Log($"{name}: Player avistado em distância {distToTarget:F2}");
                        lastSeePlayer = true;
                    }
                    return true;
                }
            }
        }

        if (lastSeePlayer)
        {
            Debug.Log($"{name}: Perdeu o player de vista.");
            lastSeePlayer = false;
        }

        return false;
    }

    private Vector3 GetViewDirection()
    {
        if (agent != null && agent.velocity.sqrMagnitude > 0.01f)
        {
            return agent.velocity.normalized;
        }
        else
        {
            return transform.forward;
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

using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Chaser : MonoBehaviour
{
    [HideInInspector] public NavMeshAgent agent;

    [Tooltip("Velocidade de rotação suave ao mirar no alvo")]
    public float rotationSpeed = 10f;

    [Tooltip("Distância mínima antes de parar completamente")]
    public float stopDistanceBuffer = 0.1f;

    private Transform currentTarget;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = true;
        agent.updateRotation = false;
        agent.stoppingDistance = 1.2f; // pode ser sobrescrito
    }

    private void Update()
    {
        if (currentTarget == null || !agent.isActiveAndEnabled)
            return;

        // Distância até o alvo
        float dist = Vector3.Distance(transform.position, currentTarget.position);

        // Continua perseguindo enquanto estiver fora da distância de parada
        if (dist > agent.stoppingDistance + stopDistanceBuffer)
        {
            if (!agent.pathPending)
                agent.SetDestination(currentTarget.position);
        }
        else
        {
            StopChasing();
        }

        RotateTowards(currentTarget.position);
    }

    /// <summary>
    /// Inicia perseguição ao alvo.
    /// </summary>
    public void ChaseTarget(Transform target)
    {
        if (target == null) return;

        currentTarget = target; // 🔥 FIX: salva o alvo
        agent.isStopped = false;
        agent.updateRotation = false;
        agent.SetDestination(target.position);
        RotateTowards(target.position);
    }

    /// <summary>
    /// Para o movimento completamente e limpa o destino.
    /// </summary>
    public void StopChasing()
    {
        agent.ResetPath();
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
    }

    /// <summary>
    /// Retorna true se chegou suficientemente perto do alvo.
    /// </summary>
    public bool HasReachedTarget(float stopDistance = -1f)
    {
        if (currentTarget == null) return false;

        if (stopDistance <= 0) stopDistance = agent.stoppingDistance;
        return Vector3.Distance(transform.position, currentTarget.position) <= stopDistance;
    }

    /// <summary>
    /// Rotaciona suavemente na direção do alvo.
    /// </summary>
    private void RotateTowards(Vector3 worldPosition)
    {
        Vector3 direction = (worldPosition - transform.position).normalized;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion lookRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * rotationSpeed);
        }
    }

    /// <summary>
    /// Para uso pelo DogAI (para pegar o alvo atual).
    /// </summary>
    public Transform GetCurrentTarget()
    {
        return currentTarget;
    }

    /// <summary>
    /// Para uso pelo DogAI ao mudar de estado.
    /// </summary>
    public void ClearTarget()
    {
        currentTarget = null;
    }
}

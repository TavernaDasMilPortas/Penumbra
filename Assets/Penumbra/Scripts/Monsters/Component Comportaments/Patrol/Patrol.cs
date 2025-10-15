using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Linq;

public enum PatrolMode { Random, Sequential, PingPong }

[RequireComponent(typeof(NavMeshAgent))]
public class Patrol : MonoBehaviour
{
    [Header("Configuração de Patrulha")]
    public PatrolPointGroup patrolGroup;
    public PatrolMode patrolMode = PatrolMode.Random;
    public float arrivalThreshold = 1.0f;
    public bool startFromClosest = true;
    public float waitTimeAtPoint = 0f;
    public float rotationSpeed = 5f;

    private NavMeshAgent agent;
    private Transform currentTarget;
    private Transform lastTarget;
    private int sequentialIndex = 0;
    private bool pingPongForward = true;

    private Transform[] patrolPoints;
    private EnemyBase enemyBase;

    // Controle de espera
    private float waitTimer = 0f;
    private bool isWaiting = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyBase = GetComponent<EnemyBase>();

        agent.updateRotation = false;
        agent.stoppingDistance = arrivalThreshold;
        agent.autoBraking = true;
    }

    private void OnEnable()
    {
        // 🔹 Garante que escutamos o evento do EnemyBase
        if (enemyBase != null)
            enemyBase.OnAgentReady.AddListener(OnAgentReady);
    }

    private void OnDisable()
    {
        if (enemyBase != null)
            enemyBase.OnAgentReady.RemoveListener(OnAgentReady);
    }

    private void Start()
    {
        // 🔹 Caso o grupo seja atribuído via Inspector
        if (enemyBase.agentReady && patrolGroup != null)
        {
            InitializePatrol(patrolGroup);
        }
    }

    /// <summary>
    /// Chamado quando o EnemyBase sinaliza que o agente está pronto.
    /// </summary>
    private void OnAgentReady()
    {
        if (patrolGroup != null)
        {
            InitializePatrol(patrolGroup);
            Debug.Log($"[{name}] Patrulha iniciada após agentReady com grupo {patrolGroup.name}.");
        }
        else
        {
            Debug.LogWarning($"[{name}] AgentReady, mas PatrolGroup ainda não definido!");
        }
    }

    /// <summary>
    /// Define o grupo de patrulha, mas só inicia quando o agente estiver pronto.
    /// </summary>
    public void SetPatrolGroup(PatrolPointGroup group)
    {
        patrolGroup = group;
        Debug.Log($"[{name}] PatrolGroup {group.name} atribuído (aguardando agentReady).");
    }

    /// <summary>
    /// Inicializa efetivamente a patrulha.
    /// </summary>
    public void InitializePatrol(PatrolPointGroup group)
    {
        if (!enemyBase.agentReady)
        {
            Debug.Log($"[{name}] Tentativa de iniciar patrulha antes de agentReady, abortando.");
            return;
        }

        if (group == null || group.patrolPoints == null || group.patrolPoints.Length == 0)
        {
            Debug.LogWarning($"[{name}] Grupo de patrulha inválido!");
            enabled = false;
            return;
        }

        patrolGroup = group;
        patrolPoints = patrolGroup.patrolPoints;

        // Escolhe o ponto inicial
        if (startFromClosest)
            currentTarget = GetClosestPoint(transform.position, patrolPoints.ToList());
        else
            currentTarget = patrolPoints[0];

        MoveTo(currentTarget);
    }

    private void Update()
    {
        if (!enemyBase.agentReady || currentTarget == null || patrolPoints == null || patrolPoints.Length == 0)
            return;

        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                isWaiting = false;
                ChooseNextPoint();
            }
            return;
        }

        if (!agent.pathPending && HasReachedTarget())
        {
            if (waitTimeAtPoint > 0f)
            {
                isWaiting = true;
                waitTimer = waitTimeAtPoint;
                agent.isStopped = true;
                agent.velocity = Vector3.zero;
            }
            else
            {
                ChooseNextPoint();
            }
        }
        else
        {
            agent.isStopped = false;
        }

        RotateTowardsTarget();
    }

    private void ChooseNextPoint()
    {
        lastTarget = currentTarget;

        switch (patrolMode)
        {
            case PatrolMode.Random:
                var candidates = patrolPoints.Where(p => p != lastTarget).ToList();
                if (candidates.Count > 0)
                    currentTarget = candidates[Random.Range(0, candidates.Count)];
                break;

            case PatrolMode.Sequential:
                sequentialIndex = (sequentialIndex + 1) % patrolPoints.Length;
                currentTarget = patrolPoints[sequentialIndex];
                break;

            case PatrolMode.PingPong:
                if (pingPongForward)
                {
                    sequentialIndex++;
                    if (sequentialIndex >= patrolPoints.Length - 1)
                    {
                        sequentialIndex = patrolPoints.Length - 1;
                        pingPongForward = false;
                    }
                }
                else
                {
                    sequentialIndex--;
                    if (sequentialIndex <= 0)
                    {
                        sequentialIndex = 0;
                        pingPongForward = true;
                    }
                }
                currentTarget = patrolPoints[sequentialIndex];
                break;
        }

        MoveTo(currentTarget);
    }

    private void MoveTo(Transform target)
    {
        if (agent.isActiveAndEnabled && target != null)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
        }
    }

    private void RotateTowardsTarget()
    {
        if (agent.velocity.sqrMagnitude > 0.01f && currentTarget != null)
        {
            Vector3 direction = (agent.steeringTarget - transform.position).normalized;
            direction.y = 0;

            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }

    private bool HasReachedTarget()
    {
        return agent.remainingDistance <= agent.stoppingDistance &&
               (!agent.hasPath || agent.velocity.sqrMagnitude < 0.05f);
    }

    private Transform GetClosestPoint(Vector3 position, List<Transform> points)
    {
        float minDist = Mathf.Infinity;
        List<Transform> closest = new List<Transform>();

        foreach (var p in points)
        {
            float dist = Vector3.Distance(position, p.position);
            if (dist < minDist - 0.01f)
            {
                minDist = dist;
                closest.Clear();
                closest.Add(p);
            }
            else if (Mathf.Abs(dist - minDist) < 0.01f)
            {
                closest.Add(p);
            }
        }

        return closest[Random.Range(0, closest.Count)];
    }
}

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

    public event System.Action OnPatrolStarted;
    public event System.Action OnPatrolStopped;
    public event System.Action<Transform> OnPatrolPointReached;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyBase = GetComponent<EnemyBase>();

        agent.updateRotation = false;
        agent.stoppingDistance = arrivalThreshold;
        agent.autoBraking = true;

        // 🚫 Impede Update de rodar antes de termos patrolGroup
        enabled = false;
    }

    private void OnEnable()
    {
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
        // Não inicia aqui. Aguarda SetPatrolGroup()
    }

    private void OnAgentReady()
    {
        if (patrolGroup != null)
        {
            InitializePatrol(patrolGroup);
            enabled = true;
        }
    }

    // =====================================================================
    // ASSIGNACIÓN DO GRUPO
    // =====================================================================
    public void SetPatrolGroup(PatrolPointGroup group)
    {
        patrolGroup = group;

        if (enemyBase != null && enemyBase.agentReady)
        {
            InitializePatrol(group);
            enabled = true;
        }
    }

    // =====================================================================
    // INÍCIO DA PATRULHA
    // =====================================================================
    public void InitializePatrol(PatrolPointGroup group)
    {
        if (!enemyBase.agentReady)
            return;

        if (group == null)
        {
            Debug.LogError(
                $"[Patrol] {name}: PatrolGroup é NULL!\n" +
                $"- Isso significa que o spawner NÃO chamou patrol.SetPatrolGroup(group)\n" +
                $"- OU você esqueceu de vincular o grupo no Inspector.\n"
            );
            enabled = false;
            return;
        }

        if (group.patrolPoints == null)
        {
            Debug.LogError(
                $"[Patrol] {name}: PatrolGroup '{group.name}' possui patrolPoints = NULL!\n" +
                "- Isso normalmente significa que o ScriptableObject foi criado, mas nenhum ponto foi atribuído no array.\n"
            );
            enabled = false;
            return;
        }

        if (group.patrolPoints.Length == 0)
        {
            Debug.LogError(
                $"[Patrol] {name}: PatrolGroup '{group.name}' possui patrolPoints VAZIO (0 pontos)!\n" +
                "- Você deve adicionar pelo menos 1 ponto de patrulha no ScriptableObject."
            );
            enabled = false;
            return;
        }

        // Verifica se existem pontos nulos dentro do array
        bool hasNull = false;
        for (int i = 0; i < group.patrolPoints.Length; i++)
        {
            if (group.patrolPoints[i] == null)
            {
                Debug.LogError(
                    $"[Patrol] {name}: PatrolGroup '{group.name}' possui o ponto de índice {i} = NULL!\n" +
                    $"- Abra o ScriptableObject e substitua o ponto faltando."
                );
                hasNull = true;
            }
        }

        if (hasNull)
        {
            enabled = false;
            return;
        }


        patrolGroup = group;
        patrolPoints = patrolGroup.patrolPoints;

        currentTarget = startFromClosest ?
            GetClosestPoint(transform.position, patrolPoints.ToList()) :
            patrolPoints[0];

        MoveTo(currentTarget);
    }

    // =====================================================================
    // UPDATE — agora SEM NULLS
    // =====================================================================
    private void Update()
    {
        if (!enemyBase.agentReady) return;
        if (patrolPoints == null || patrolPoints.Length == 0) return;
        if (currentTarget == null) return;

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
            OnPatrolPointReached?.Invoke(currentTarget);

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
        if (!agent.isActiveAndEnabled || target == null)
            return;

        agent.isStopped = false;
        agent.SetDestination(target.position);
    }

    // =====================================================================
    // ROTACIONAMENTO SEGURO
    // =====================================================================
    private void RotateTowardsTarget()
    {
        if (!agent.hasPath || agent.pathPending)
            return;

        if (currentTarget == null)
            return;

        Vector3 steering = agent.steeringTarget;

        if (float.IsNaN(steering.x))
            return;

        Vector3 direction = (steering - transform.position).normalized;
        direction.y = 0;

        if (direction.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
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

    public void StartPatrol()
    {
        enabled = true;
        if (currentTarget == null && patrolPoints != null && patrolPoints.Length > 0)
            currentTarget = startFromClosest ?
                GetClosestPoint(transform.position, patrolPoints.ToList()) :
                patrolPoints[0];

        MoveTo(currentTarget);
        OnPatrolStarted?.Invoke();
    }

    public void StopPatrol()
    {
        agent.isStopped = true;
        OnPatrolStopped?.Invoke();
    }
}

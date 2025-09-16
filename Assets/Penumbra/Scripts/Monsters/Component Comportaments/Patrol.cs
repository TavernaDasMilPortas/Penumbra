using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Linq;

public enum PatrolMode { Random, Sequential, PingPong }

[RequireComponent(typeof(NavMeshAgent))]
public class Patrol : MonoBehaviour
{
    [Header("Configuração de Patrulha")]
    public PatrolPointGroup patrolGroup; // pode ser setado no inspector ou em runtime
    public PatrolMode patrolMode = PatrolMode.Random;
    public float arrivalThreshold = 1.0f;
    public bool startFromClosest = true;
    public float waitTimeAtPoint = 0f; // tempo parado em cada ponto

    private NavMeshAgent agent;
    private Transform currentTarget;
    private Transform lastTarget;
    private int sequentialIndex = 0;
    private bool pingPongForward = true;

    private Transform[] patrolPoints;

    // Controle de espera
    private float waitTimer = 0f;
    private bool isWaiting = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        if (patrolGroup != null)
        {
            InitializePatrol(patrolGroup);
        }
    }

    /// <summary>
    /// Configura o grupo de patrulha em runtime (pode ser chamado no spawn).
    /// </summary>
    public void InitializePatrol(PatrolPointGroup group)
    {
        if (group == null || group.patrolPoints == null || group.patrolPoints.Length == 0)
        {
            Debug.LogWarning($"{name}: Grupo de patrulha inválido!");
            enabled = false;
            return;
        }

        patrolGroup = group;
        patrolPoints = patrolGroup.patrolPoints;

        // Define o primeiro ponto
        if (startFromClosest)
        {
            currentTarget = GetClosestPoint(transform.position, patrolPoints.ToList());
        }
        else
        {
            currentTarget = patrolPoints[0];
        }

        MoveTo(currentTarget);
    }

    /// <summary>
    /// Troca o grupo de patrulha em runtime.
    /// </summary>
    public void SetPatrolGroup(PatrolPointGroup group)
    {
        InitializePatrol(group);
    }

    private void Update()
    {
        if (currentTarget == null || patrolPoints == null || patrolPoints.Length == 0) return;

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

        if (!agent.pathPending && agent.remainingDistance <= arrivalThreshold)
        {
            if (waitTimeAtPoint > 0f)
            {
                isWaiting = true;
                waitTimer = waitTimeAtPoint;
                agent.ResetPath(); // para não tentar andar no "nada"
            }
            else
            {
                ChooseNextPoint();
            }
        }
    }

    private void ChooseNextPoint()
    {
        lastTarget = currentTarget;

        switch (patrolMode)
        {
            case PatrolMode.Random:
                List<Transform> candidates = patrolPoints.Where(p => p != lastTarget).ToList();
                if (candidates.Count > 0)
                {
                    currentTarget = candidates[Random.Range(0, candidates.Count)];
                }
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
            agent.SetDestination(target.position);
        }
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

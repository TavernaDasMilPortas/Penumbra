using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Chaser : MonoBehaviour
{
    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void ChaseTarget(Transform target)
    {
        if (target == null) return;
        agent.isStopped = false;
        agent.SetDestination(target.position);
    }

    public void StopChasing()
    {
        agent.isStopped = true;
    }

    public bool HasReachedTarget(float stopDistance = 1f)
    {
        if (!agent.pathPending && agent.remainingDistance <= stopDistance)
            return true;

        return false;
    }
}

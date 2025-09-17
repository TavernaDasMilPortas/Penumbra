using UnityEngine;

[RequireComponent(typeof(Chaser))]
[RequireComponent(typeof(Vision))]
[RequireComponent(typeof(Patrol))]
public class Hunter : MonoBehaviour
{
    private enum State { Patrolling, Chasing }
    private State currentState = State.Patrolling;

    private Vision vision;
    private Chaser chaser;
    private Patrol patrol;

    [Header("Configurações de Hunter")]
    public float lostPlayerCooldown = 3f; // tempo que ele continua procurando após perder visão
    private float lostTimer = 0f;

    private Transform player;

    private void Awake()
    {
        vision = GetComponent<Vision>();
        chaser = GetComponent<Chaser>();
        patrol = GetComponent<Patrol>();
    }

    private void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
            player = p.transform;
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Patrolling:
                PatrolBehaviour();
                break;

            case State.Chasing:
                ChaseBehaviour();
                break;
        }
    }

    private void PatrolBehaviour()
    {
        patrol.enabled = true;

        if (vision.CanSeePlayer())
        {
            currentState = State.Chasing;
            patrol.enabled = false;
        }
    }

    private void ChaseBehaviour()
    {
        if (player == null) return;

        chaser.ChaseTarget(player);

        if (vision.CanSeePlayer())
        {
            lostTimer = 0f; // reset porque ainda vê
        }
        else
        {
            lostTimer += Time.deltaTime;
            if (lostTimer >= lostPlayerCooldown)
            {
                // perdeu o player, volta para patrulha
                currentState = State.Patrolling;
                chaser.StopChasing();
                patrol.enabled = true;
            }
        }
    }
}

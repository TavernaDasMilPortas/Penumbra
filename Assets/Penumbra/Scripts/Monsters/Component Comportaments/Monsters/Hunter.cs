using UnityEngine;

[RequireComponent(typeof(Patrol))]
[RequireComponent(typeof(Chaser))]
public class Hunter : EnemyBase
{
    [Header("Hunter Settings")]
    public float chaseLostTime = 3f;
    public float chaseStopDistance = 1.2f;

    private Patrol patrol;
    private Chaser chaser;
    [SerializeField] private Vision vision;

    private float lostTimer = 0f;
    private bool isChasing = false;
    public float originalAgentSpeed = 20f; // armazena velocidade original do agente
    protected override void Awake()
    {
        base.Awake();
        patrol = GetComponent<Patrol>();
        chaser = GetComponent<Chaser>();

        // Salva a velocidade original do agente

    }

    protected override void Start()
    {
        base.Start();
        OnAgentReady.AddListener(HandleAgentReady);
        chaser.agent.stoppingDistance = chaseStopDistance;
    }

        private void HandleAgentReady()
    {
        Debug.Log($"{name}: agente pronto, comportamento iniciado.");
        chaser.agent.speed = originalAgentSpeed;
        // agora o Hunter começa a operar
    }


    private void Update()
    {
        if (!agentReady) return; // só age quando estiver pronto
       

        if (vision.CanSeePlayer())
        {
            if (!isChasing) Debug.Log($"{name}: jogador avistado!");
            isChasing = true;
            lostTimer = chaseLostTime;

            // opcional: aumentar a velocidade do agente ao perseguir
            chaser.agent.speed = originalAgentSpeed * 1.5f;
        }
        else if (isChasing)
        {
            lostTimer -= Time.deltaTime;
            if (lostTimer <= 0f)
            {
                isChasing = false;
                Debug.Log($"{name}: jogador perdido, retornando à patrulha.");

                // 🔹 restaura a velocidade original do agente
                chaser.agent.speed = originalAgentSpeed;
            }
        }

        if (isChasing && vision.target != null)
        {
            chaser.ChaseTarget(vision.target);

            if (Vector3.Distance(transform.position, vision.target.position) <= chaseStopDistance)
            {
                chaser.StopChasing();

                // Faz o inimigo olhar para o jogador ao parar
                Vector3 lookDir = (vision.target.position - transform.position).normalized;
                lookDir.y = 0f;
                if (lookDir.sqrMagnitude > 0.001f)
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * 10f);

                // lógica de ataque futura
            }
        }
    }

}

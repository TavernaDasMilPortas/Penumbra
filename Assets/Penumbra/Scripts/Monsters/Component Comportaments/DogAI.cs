using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Patrol))]
[RequireComponent(typeof(RoomTracker))]
[RequireComponent(typeof(DogAudio))]
public class DogAI : EnemyBase
{
    public enum State
    {
        Searching,
        Spotted,
        PreparingCharge,
        Charging,
        Cooldown,
        Eating,
        Kill
    }

    [Header("Distâncias")]
    public float detectionRadius = 12f;
    public float chargePrepareDistance = 7f;
    public float chargeSpeed = 18f;
    public float normalSpeed = 6f;

    [Header("Tempos")]
    public float prepareTime = 0.8f;
    public float cooldownTime = 1.5f;

    [Header("Referência ao Pote via PointReference")]
    public PointReference bowlPoint;

    [Header("Objetos resolvidos automaticamente")]
    public Transform foodBowl;
    public DogFoodBowlInteractable bowlInteractable;

    [Header("Outras referências")]
    public Patrol patrol;
    public RoomTracker roomTracker;

    [Header("Ponto da Boca (trigger de kill e medição de distância)")]
    public Transform mouthPoint;

    private Transform player;
    private DogAudio audioSystem;

    private State currentState;
    private float stateTimer;
    private Vector3 chargeDirection;

    [Header("Componentes")]
    public Vision vision;
    public State CurrentState => currentState;


    // ============================================================
    // UTILS — posição da boca
    // ============================================================
    private Vector3 GetMouthPos()
    {
        return mouthPoint != null ? mouthPoint.position : transform.position;
    }


    // ============================================================
    // TRIGGER DE KILL — classe interna
    // ============================================================
    private class DogAIMouthTrigger : MonoBehaviour
    {
        public DogAI owner;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            // Kill só permitido se estiver atacando
            //if (owner.currentState == State.Charging ||
            //    owner.currentState == State.PreparingCharge)
            //{
                Debug.Log("[DogAI] Kill por boca → Player colidiu com mouth trigger");
                owner.SetState(State.Kill);
            //}
        }
    }


    // ============================================================
    // AWAKE
    // ============================================================
    protected override void Awake()
    {
        base.Awake();

        patrol = GetComponent<Patrol>();
        roomTracker = GetComponent<RoomTracker>();
        audioSystem = GetComponent<DogAudio>();
        vision = GetComponentInChildren<Vision>();

        GameObject pObj = GameObject.FindGameObjectWithTag("Player");
        if (pObj != null)
        {
            player = pObj.transform;
            Debug.Log($"[DogAI] Player encontrado: {player.name}");
        }
        else
            Debug.LogError("[DogAI] Nenhum objeto com tag 'Player' encontrado!");


        // ===============================
        // VALIDAR MOUTHPOINT
        // ===============================
        if (mouthPoint != null)
        {
            Collider col = mouthPoint.GetComponent<Collider>();
            if (col == null)
                Debug.LogWarning("[DogAI] mouthPoint NÃO possui collider — kill não funcionará!");
            else if (!col.isTrigger)
                Debug.LogWarning("[DogAI] mouthPoint collider existe mas isTrigger = false!");

            var trigger = mouthPoint.gameObject.AddComponent<DogAIMouthTrigger>();
            trigger.owner = this;
        }
    }


    // ============================================================
    // START
    // ============================================================
    protected override void Start()
    {
        base.Start();

        ResolveFoodBowlPoint();

        patrol.enabled = false;

        if (agentReady)
            HandleAgentReady();
        else
            OnAgentReady.AddListener(HandleAgentReady);
    }

    private void HandleAgentReady()
    {
        patrol.enabled = true;

        if (patrol.patrolGroup != null)
            patrol.SetPatrolGroup(patrol.patrolGroup);

        SetState(State.Searching);
    }


    // ============================================================
    // RESTAURADO — RESOLVE FOOD BOWL
    // ============================================================
    private void ResolveFoodBowlPoint()
    {
        if (bowlPoint == null || string.IsNullOrEmpty(bowlPoint.pointName))
        {
            Debug.LogError($"[DogAI] bowlPoint não definido.");
            return;
        }

        if (PointManager.Instance == null)
        {
            Debug.LogError("[DogAI] PointManager não encontrado.");
            return;
        }

        Debug.Log($"[DogAI] Resolvendo ponto do pote: {bowlPoint.pointName}");

        Point point = PointManager.Instance.GetPointByName(bowlPoint.pointName);

        if (point == null)
        {
            Debug.LogError($"[DogAI] Nenhum Point chamado '{bowlPoint.pointName}' foi encontrado.");
            return;
        }

        ItemInstance inst = point.selfTransform.GetComponentInChildren<ItemInstance>();

        if (inst == null)
        {
            Debug.LogError($"[DogAI] Nenhum ItemInstance encontrado no point '{bowlPoint.pointName}'.");
            return;
        }

        foodBowl = inst.transform;
        bowlInteractable = inst.GetComponent<DogFoodBowlInteractable>();

        if (bowlInteractable == null)
            Debug.LogWarning($"[DogAI] O objeto no point '{bowlPoint.pointName}' não possui DogFoodBowlInteractable.");

        Debug.Log($"[DogAI] FoodBowl resolvido → {foodBowl.name}");
    }


    // ============================================================
    // UPDATE
    // ============================================================
    private void Update()
    {
        if (!agentReady) return;

        switch (currentState)
        {
            case State.Searching: UpdateSearching(); break;
            case State.Spotted: UpdateSpotted(); break;
            case State.PreparingCharge: UpdatePreparingCharge(); break;
            case State.Charging: UpdateCharging(); break;
            case State.Cooldown: UpdateCooldown(); break;
            case State.Eating: UpdateEating(); break;
            case State.Kill: UpdateKill(); break;
        }
    }


    // ============================================================
    // STATE MACHINE — com logs
    // ============================================================
    private void SetState(State newState)
    {
        Debug.Log($"[DogAI] Estado: {currentState} → {newState}");

        currentState = newState;
        stateTimer = 0f;

        switch (newState)
        {
            case State.Searching:
                agent.isStopped = false;
                agent.speed = normalSpeed;
                patrol?.StartPatrol();
                break;

            case State.Spotted:
                patrol?.StopPatrol();
                agent.isStopped = false;
                audioSystem?.PlayBark();
                break;

            case State.PreparingCharge:
                agent.isStopped = true;
                audioSystem?.PlayGrowl();
                break;

            case State.Charging:
                agent.isStopped = false;
                agent.speed = chargeSpeed;
                audioSystem?.PlayCharge();
                break;

            case State.Cooldown:
                agent.isStopped = true;
                agent.speed = normalSpeed;
                break;

            case State.Eating:
                agent.isStopped = false;
                audioSystem?.PlayEating();
                break;

            case State.Kill:
                agent.isStopped = true;
                break;
        }
    }


    // ============================================================
    // STATES IMPLEMENTATION
    // ============================================================

    private void UpdateSearching()
    {
        if (vision != null && vision.CanSeePlayer())
        {
            Debug.Log("[DogAI] Player detectado → Spotted");
            SetState(State.Spotted);
            return;
        }

        if (bowlInteractable != null && !bowlInteractable.isEmpyt)
        {
            var loc = SessionManager.Instance.GetLocation(transform.position);
            if (loc.Item2 != null && loc.Item2.roomName == "Lavanderia")
            {
                Debug.Log("[DogAI] Bowl cheio e dog na lavanderia → Eating");
                SetState(State.Eating);
            }
        }
    }


    private void UpdateSpotted()
    {
        if (!vision.CanSeePlayer())
        {
            Debug.Log("[DogAI] Perdeu visão → Searching");
            SetState(State.Searching);
            return;
        }

        agent.SetDestination(player.position);

        Vector3 dir = (player.position - GetMouthPos()).normalized;
        dir.y = 0;
        if (dir.sqrMagnitude > 0.1f)
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(dir),
                Time.deltaTime * 8f
            );

        if (Vector3.Distance(GetMouthPos(), player.position) <= chargePrepareDistance)
        {
            Debug.Log("[DogAI] Distância ideal → PreparingCharge");
            SetState(State.PreparingCharge);
        }
    }


    private void UpdatePreparingCharge()
    {
        if (!vision.CanSeePlayer())
        {
            Debug.Log("[DogAI] Perdeu visão durante preparo → Searching");
            SetState(State.Searching);
            return;
        }

        stateTimer += Time.deltaTime;

        chargeDirection = (player.position - GetMouthPos()).normalized;
        chargeDirection.y = 0;

        if (stateTimer >= prepareTime)
        {
            Debug.Log("[DogAI] Preparação completa → Charging");
            SetState(State.Charging);
        }
    }


    private void UpdateCharging()
    {
        agent.SetDestination(GetMouthPos() + chargeDirection * 2f);
        transform.rotation = Quaternion.LookRotation(chargeDirection);

        if (!vision.CanSeePlayer())
        {
            Debug.Log("[DogAI] Perdeu visão → Cooldown");
            SetState(State.Cooldown);
        }
    }


    private void UpdateCooldown()
    {
        stateTimer += Time.deltaTime;
        if (stateTimer >= cooldownTime)
        {
            Debug.Log("[DogAI] Cooldown terminado → Searching");
            SetState(State.Searching);
        }
    }


    private void UpdateEating()
    {
        if (foodBowl == null)
        {
            Debug.LogWarning("[DogAI] Eating mas foodBowl é null!");
            return;
        }

        agent.SetDestination(foodBowl.position);

        if (Vector3.Distance(GetMouthPos(), foodBowl.position) < 1.5f)
        {
            agent.isStopped = true;

            Vector3 look = foodBowl.position - transform.position;
            look.y = 0;
            transform.rotation = Quaternion.LookRotation(look);
        }
    }


    private void UpdateKill()
    {
        Debug.Log("[DogAI] Player morto.");
        NightManager.Instance.OnPlayerDeath();
    }


    // ============================================================
    // DEBUG STATE
    // ============================================================
    public void DebugForceState(State newState)
    {
        if (!agentReady)
        {
            Debug.LogWarning("[DogAI] agentReady = false, não pode forçar estado.");
            return;
        }

        DebugForce_Internal(newState);
    }

    private void DebugForce_Internal(State s)
    {
        typeof(DogAI)
            .GetMethod("SetState", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .Invoke(this, new object[] { s });
    }
}

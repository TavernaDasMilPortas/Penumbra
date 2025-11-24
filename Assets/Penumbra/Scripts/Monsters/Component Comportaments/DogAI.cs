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

    [Header("CONFIGURAÇÃO DO KILL")]
    [Tooltip("Colisor que mata o player ao ser atingido. Deve ser TRIGGER.")]
    public Collider killCollider;

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

    [Header("Ponto da Boca")]
    public Transform mouthPoint;

    private Transform player;
    private DogAudio audioSystem;
    private Vision vision;

    private State currentState;
    private float stateTimer;
    private Vector3 chargeDirection;
    private Vector3 lastChargeDir;
    private Animator anim;

    public State CurrentState => currentState;

    protected override void Awake()
    {
        base.Awake();

        patrol = GetComponent<Patrol>();
        roomTracker = GetComponent<RoomTracker>();
        audioSystem = GetComponent<DogAudio>();
        vision = GetComponentInChildren<Vision>();
        anim = GetComponentInChildren<Animator>();

        GameObject pObj = GameObject.FindGameObjectWithTag("Player");
        if (pObj != null)
            player = pObj.transform;

        if (killCollider == null)
            Debug.LogWarning("[DogAI] KillCollider NÃO atribuído!");

        if (killCollider != null && !killCollider.isTrigger)
            Debug.LogError("[DogAI] O KillCollider deve ter 'isTrigger = true'!");
    }

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

    private void OnEnable()
    {
        if (killCollider != null)
        {
            KillTriggerListener listener = killCollider.gameObject.AddComponent<KillTriggerListener>();
            listener.owner = this;
        }
    }

    private class KillTriggerListener : MonoBehaviour
    {
        public DogAI owner;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            Debug.Log("[DogAI] KILL TRIGGER → Player atingido durante o Charge!");
            owner.SetState(State.Kill);
        }
    }

    private void HandleAgentReady()
    {
        patrol.enabled = true;

        if (patrol.patrolGroup != null)
            patrol.SetPatrolGroup(patrol.patrolGroup);

        SetState(State.Searching);
    }

    private void Update()
    {
        if (!agentReady) return;

        UpdateAnimation();

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

    private void UpdateAnimation()
    {
        if (anim == null || agent == null)
            return;

        float speed = agent.velocity.magnitude;

        anim.SetFloat("Speed", speed);
        anim.SetBool("IsPreparing", currentState == State.PreparingCharge);
        anim.SetBool("IsCharging", currentState == State.Charging);

        float baseSpeed = 1f;

        if (currentState == State.Charging)
            baseSpeed = 2f;
        else if (speed > normalSpeed * 0.8f)
            baseSpeed = 1.4f;

        if (currentState == State.PreparingCharge)
            baseSpeed = 1f;

        anim.speed = baseSpeed;
    }

    private void SetState(State newState)
    {
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
                break;

            case State.PreparingCharge:
                agent.isStopped = true;

                // 🔥 FORÇA A ENTRADA DA ANIMAÇÃO AttackPrepare
                if (anim != null)
                {
                    Debug.Log("[DogAI] Tocando animação AttackPrepare");
                    anim.CrossFade("AttackPrepare", 0.1f);
                }
                break;

            case State.Charging:
                agent.isStopped = false;
                agent.speed = chargeSpeed;
                break;

            case State.Cooldown:
                agent.isStopped = true;
                agent.speed = normalSpeed;
                break;

            case State.Eating:
                agent.isStopped = false;
                break;

            case State.Kill:
                agent.isStopped = true;
                break;
        }
    }

    private void UpdateSearching()
    {
        if (vision != null && vision.CanSeePlayer())
        {
            SetState(State.Spotted);
            return;
        }
    }

    private void UpdateSpotted()
    {
        if (!vision.CanSeePlayer())
        {
            SetState(State.Searching);
            return;
        }

        agent.SetDestination(player.position);

        if (Vector3.Distance(GetMouthPos(), player.position) <= chargePrepareDistance)
        {
            SetState(State.PreparingCharge);
        }
    }

    private void UpdatePreparingCharge()
    {
        agent.isStopped = true;
        stateTimer += Time.deltaTime;

        chargeDirection = (player.position - GetMouthPos()).normalized;
        chargeDirection.y = 0;

        if (stateTimer >= prepareTime)
        {
            lastChargeDir = chargeDirection;
            SetState(State.Charging);
        }
    }

    private void UpdateCharging()
    {
        agent.SetDestination(GetMouthPos() + lastChargeDir * 2f);

        if (!vision.CanSeePlayer())
        {
            SetState(State.Cooldown);
        }
    }

    private void UpdateCooldown()
    {
        stateTimer += Time.deltaTime;
        if (stateTimer >= cooldownTime)
        {
            SetState(State.Searching);
        }
    }

    private void UpdateEating()
    {
        if (foodBowl == null) return;

        agent.SetDestination(foodBowl.position);
    }

    private void UpdateKill()
    {
        NightManager.Instance.OnPlayerDeath();
    }

    private Vector3 GetMouthPos()
    {
        return mouthPoint != null ? mouthPoint.position : transform.position;
    }

    private void ResolveFoodBowlPoint()
    {
        if (bowlPoint == null) return;

        Point point = PointManager.Instance.GetPointByName(bowlPoint.pointName);
        if (point == null) return;

        ItemInstance inst = point.selfTransform.GetComponentInChildren<ItemInstance>();
        if (inst == null) return;

        foodBowl = inst.transform;
        bowlInteractable = inst.GetComponent<DogFoodBowlInteractable>();
    }

    public void DebugForceState(State newState)
    {
        if (!agentReady)
        {
            Debug.LogWarning("[DogAI] agentReady = false.");
            return;
        }

        typeof(DogAI)
            .GetMethod("SetState", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .Invoke(this, new object[] { newState });
    }
}

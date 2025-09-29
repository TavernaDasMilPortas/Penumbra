using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Controla o comportamento de patrulha e perseguição de um inimigo.
/// </summary>
[RequireComponent(typeof(Patrol))]
[RequireComponent(typeof(Chaser))]
public class Hunter : MonoBehaviour
{
    [Header("Configuração do Hunter")]
    public float chaseLostTime = 3f; // tempo que ele continua perseguindo depois de perder a visão
    public float chaseStopDistance = 1.2f; // distância para considerar que chegou no player

    private Patrol patrol;
    public Vision vision;
    private Chaser chaser;

    public Transform player;
    private float lostTimer = 0f;
    private bool isChasing = false;

    private void Awake()
    {
        patrol = GetComponent<Patrol>();
        chaser = GetComponent<Chaser>();
    }

    private void Update()
    {
        player = vision != null ? GetTargetFromVision() : null;

        if (vision != null && vision.CanSeePlayer())
        {
            Debug.Log($"{name}: Player avistado, começando perseguição!");
            StartChasing();
            lostTimer = chaseLostTime;
        }
        else if (isChasing)
        {
            lostTimer -= Time.deltaTime;
            Debug.Log($"{name}: Não vejo o player, timer = {lostTimer:F2}");

            if (lostTimer <= 0f)
            {
                Debug.Log($"{name}: Perdeu o player definitivamente, voltando a patrulhar.");
                StopChasing();
            }
        }

        if (isChasing && player != null)
        {
            chaser.ChaseTarget(player);
            Debug.Log($"{name}: Perseguindo player, distância = {Vector3.Distance(transform.position, player.position):F2}");

            if (chaser.HasReachedTarget(chaseStopDistance))
            {
                Debug.Log($"{name}: Alcancei o player, parando para atacar!");
                chaser.StopChasing();
                // lógica de ataque pode ir aqui
            }
        }
    }

    private Transform GetTargetFromVision()
    {
        return vision.target;
    }

    private void StartChasing()
    {
        if (!isChasing)
        {
            Debug.Log($"{name}: Iniciando perseguição!");
            isChasing = true;
            patrol.enabled = false;
        }
    }

    private void StopChasing()
    {
        if (isChasing)
        {
            Debug.Log($"{name}: Parando perseguição, retomando patrulha.");
            isChasing = false;
            chaser.StopChasing();
            patrol.enabled = true;
        }
    }
}

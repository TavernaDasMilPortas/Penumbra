using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Controla o comportamento de patrulha e persegui��o de um inimigo.
/// </summary>
[RequireComponent(typeof(Patrol))]
[RequireComponent(typeof(Chaser))]
public class Hunter : MonoBehaviour
{
    [Header("Configura��o do Hunter")]
    public float chaseLostTime = 3f; // tempo que ele continua perseguindo depois de perder a vis�o
    public float chaseStopDistance = 1.2f; // dist�ncia para considerar que chegou no player

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
            Debug.Log($"{name}: Player avistado, come�ando persegui��o!");
            StartChasing();
            lostTimer = chaseLostTime;
        }
        else if (isChasing)
        {
            lostTimer -= Time.deltaTime;
            Debug.Log($"{name}: N�o vejo o player, timer = {lostTimer:F2}");

            if (lostTimer <= 0f)
            {
                Debug.Log($"{name}: Perdeu o player definitivamente, voltando a patrulhar.");
                StopChasing();
            }
        }

        if (isChasing && player != null)
        {
            chaser.ChaseTarget(player);
            Debug.Log($"{name}: Perseguindo player, dist�ncia = {Vector3.Distance(transform.position, player.position):F2}");

            if (chaser.HasReachedTarget(chaseStopDistance))
            {
                Debug.Log($"{name}: Alcancei o player, parando para atacar!");
                chaser.StopChasing();
                // l�gica de ataque pode ir aqui
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
            Debug.Log($"{name}: Iniciando persegui��o!");
            isChasing = true;
            patrol.enabled = false;
        }
    }

    private void StopChasing()
    {
        if (isChasing)
        {
            Debug.Log($"{name}: Parando persegui��o, retomando patrulha.");
            isChasing = false;
            chaser.StopChasing();
            patrol.enabled = true;
        }
    }
}

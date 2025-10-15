using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBase : MonoBehaviour
{
    [Header("Ground Alignment")]
    public Transform groundPoint; // Ponto que marca o "pé" do inimigo
    public float groundCheckDistance = 10f;
    public LayerMask groundMask = ~0;

    [Header("Estado")]
    public bool agentReady { get; private set; } = false;
    public UnityEvent OnAgentReady;

    protected NavMeshAgent agent;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = false; // só ativa quando estiver alinhado
    }

    protected virtual void Start()
    {
        if (groundPoint == null)
        {
            Debug.LogError($"{name}: GroundPoint não atribuído!");
            return;
        }

        StartCoroutine(AlignToGroundAndActivate());
    }

    /// <summary>
    /// Alinha o inimigo ao chão e ativa o agente quando estiver pronto.
    /// </summary>
    private IEnumerator AlignToGroundAndActivate()
    {
        bool aligned = false;
        float timeout = 3f; // segurança para evitar loops infinitos
        float timer = 0f;

        while (!aligned && timer < timeout)
        {
            timer += Time.deltaTime;

            // Raycast para baixo a partir do groundPoint
            if (Physics.Raycast(groundPoint.position + Vector3.up, Vector3.down, out RaycastHit hit, groundCheckDistance, groundMask))
            {
                // Move o inimigo para alinhar o groundPoint ao solo
                Vector3 offset = groundPoint.position - transform.position;
                transform.position = hit.point - offset;

                aligned = true;
                break;
            }

            yield return null;
        }

        if (!aligned)
        {
            Debug.LogWarning($"{name}: não conseguiu alinhar ao chão. Continuando mesmo assim.");
        }

        // Agora ativa o agente e marca como pronto
        agent.enabled = true;
        agentReady = true;
        OnAgentReady?.Invoke();

        Debug.Log($"{name}: inimigo alinhado e pronto para agir!");
    }
}

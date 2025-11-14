using UnityEngine;
using UnityEngine.Analytics;

public class RightArmController : MonoBehaviour
{
    [Header("Referências")]
    public Animator animator;

    [Tooltip("Prefab do lampião que deve ser instanciado.")]
    public GameObject lampPrefab;

    [Tooltip("Posição normal do lampião na mão.")]
    public Transform lampSocket;

    [Tooltip("Posição do lampião quando o jogador segura Q.")]
    public Transform centerViewPoint;

    [Header("Configuração")]
    public float moveSpeed = 10f;
    public float rotateSpeed = 10f;

    private GameObject lampInstance;
    private bool isVisible = false;

    // Estado interno
    private bool isHoldingUp = false;

    private void Awake()
    {
        InstantiateLamp();
        SetLampVisible(true);
    }

    private void Update()
    {
        HandleInput();
        UpdateLampMovement();
    }

    private void HandleInput()
    {
        // Aqui você deve checar seu InputManager ou GameState
        // Vou usar um exemplo genérico:

        if (GameStateManager.Instance.CurrentState != InputState.Gameplay)
            return;

        bool holdingQ = Input.GetKey(KeyCode.Q);

        if (holdingQ && !isHoldingUp)
            isHoldingUp = true;

        if (!holdingQ && isHoldingUp)
            isHoldingUp = false;
    }

    /// <summary>
    /// Move o lampião suavemente para o centro da tela ou volta para o socket.
    /// </summary>
    private void UpdateLampMovement()
    {
        if (lampInstance == null || lampSocket == null || centerViewPoint == null)
            return;

        Transform target = isHoldingUp ? centerViewPoint : lampSocket;

        // Movimento suave
        lampInstance.transform.position = Vector3.Lerp(
            lampInstance.transform.position,
            target.position,
            Time.deltaTime * moveSpeed
        );

        // Rotação suave
        lampInstance.transform.rotation = Quaternion.Slerp(
            lampInstance.transform.rotation,
            target.rotation,
            Time.deltaTime * rotateSpeed
        );
    }

    // --- Seus métodos atuais continuam iguais abaixo ---
    public void InstantiateLamp()
    {
        if (lampPrefab == null)
        {
            Debug.LogError("[RightArm] Nenhum prefab de lampião atribuído!");
            return;
        }

        if (lampSocket == null)
        {
            Debug.LogError("[RightArm] Nenhum lampSocket atribuído!");
            return;
        }

        if (lampInstance != null)
            Destroy(lampInstance);

        lampInstance = Instantiate(lampPrefab, lampSocket);
        lampInstance.transform.localPosition = Vector3.zero;
        lampInstance.transform.localRotation = Quaternion.identity;

        Debug.Log("[RightArm] Lampião instanciado no socket.");
    }

    public void SetLampVisible(bool state)
    {
        if (lampInstance == null)
            return;

        isVisible = state;
        lampInstance.SetActive(state);

        Debug.Log($"[RightArm] Lampião {(state ? "exibido" : "guardado")}");
    }
}

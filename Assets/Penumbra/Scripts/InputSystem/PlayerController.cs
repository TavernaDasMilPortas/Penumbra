using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [Header("Referências")]
    public Camera playerCamera;  // arraste a câmera do jogador aqui no inspector
    public CharacterController controller; // arraste o CharacterController do player

    [Header("Configuração Movimento")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 120f; // velocidade de rotação com A/D
    public float gravity = -9.81f;
    public float jumpHeight = 2f;

    [Header("Configuração Mouse (cabeça)")]
    public float mouseSensitivity = 200f;
    public float headYawLimit = 30f;   // limite esquerda/direita (como virar a cabeça)
    public float headPitchLimit = 60f; // limite cima/baixo

    private float headYaw = 0f;   // rotação lateral da cabeça
    private float headPitch = 0f; // rotação vertical da cabeça

    private Vector3 velocity;

    private void Awake()
    {
        Instance = this;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {

    }

    // === Movimento com W/S (frente e trás) ===
    public void HandleMovement()
    {
        float v = Input.GetAxis("Vertical"); // W/S
        Vector3 move = transform.forward * v;
        controller.Move(move * moveSpeed * Time.deltaTime);

        // gravidade
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // === Rotação com A/D ===
    public void HandleRotationKeys()
    {
        float h = Input.GetAxis("Horizontal"); // A/D
        transform.Rotate(Vector3.up * h * rotationSpeed * Time.deltaTime);
    }

    // === Movimento de cabeça com mouse ===
    public void HandleHeadLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // virar a cabeça para os lados (yaw relativo ao corpo)
        headYaw += mouseX;
        headYaw = Mathf.Clamp(headYaw, -headYawLimit, headYawLimit);

        // olhar para cima/baixo (pitch)
        headPitch -= mouseY;
        headPitch = Mathf.Clamp(headPitch, -headPitchLimit, headPitchLimit);

        // aplica rotação relativa à cabeça (localRotation)
        playerCamera.transform.localRotation = Quaternion.Euler(headPitch, headYaw, 0f);

        // garante que a câmera fique na posição da "cabeça"
        playerCamera.transform.position = transform.position + new Vector3(0, 1.6f, 0);
    }

    // === Funções de interface ===
    public void Move(float h, float v)
    {
        Vector3 move = transform.forward * v;
        controller.Move(move * moveSpeed * Time.deltaTime);
    }

    public void Stop()
    {
        velocity = Vector3.zero;
    }

    public void Interagir()
    {
        Stop();
        InteractionHandler.Instance.nearestInteractable.Interact();
    }
}

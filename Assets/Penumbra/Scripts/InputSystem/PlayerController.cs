using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [Header("Refer�ncias")]
    public Camera playerCamera;  // arraste a c�mera do jogador aqui no inspector
    public CharacterController controller; // arraste o CharacterController do player

    [Header("Configura��o Movimento")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 120f; // velocidade de rota��o com A/D
    public float gravity = -9.81f;
    public float jumpHeight = 2f;

    [Header("Configura��o Mouse (cabe�a)")]
    public float mouseSensitivity = 200f;
    public float headYawLimit = 30f;   // limite esquerda/direita (como virar a cabe�a)
    public float headPitchLimit = 60f; // limite cima/baixo

    private float headYaw = 0f;   // rota��o lateral da cabe�a
    private float headPitch = 0f; // rota��o vertical da cabe�a

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

    // === Movimento com W/S (frente e tr�s) ===
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

    // === Rota��o com A/D ===
    public void HandleRotationKeys()
    {
        float h = Input.GetAxis("Horizontal"); // A/D
        transform.Rotate(Vector3.up * h * rotationSpeed * Time.deltaTime);
    }

    // === Movimento de cabe�a com mouse ===
    public void HandleHeadLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // virar a cabe�a para os lados (yaw relativo ao corpo)
        headYaw += mouseX;
        headYaw = Mathf.Clamp(headYaw, -headYawLimit, headYawLimit);

        // olhar para cima/baixo (pitch)
        headPitch -= mouseY;
        headPitch = Mathf.Clamp(headPitch, -headPitchLimit, headPitchLimit);

        // aplica rota��o relativa � cabe�a (localRotation)
        playerCamera.transform.localRotation = Quaternion.Euler(headPitch, headYaw, 0f);

        // garante que a c�mera fique na posi��o da "cabe�a"
        playerCamera.transform.position = transform.position + new Vector3(0, 1.6f, 0);
    }

    // === Fun��es de interface ===
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

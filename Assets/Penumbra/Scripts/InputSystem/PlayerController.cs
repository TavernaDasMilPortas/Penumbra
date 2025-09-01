using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [Header("Referências")]
    public Camera playerCamera;  // arraste a câmera do jogador aqui no inspector
    public CharacterController controller; // arraste o CharacterController do player

    [Header("Configuração Movimento")]
    public float moveSpeed = 5f;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;

    [Header("Configuração Mouse")]
    public float mouseSensitivity = 200f;
    private float xRotation = 0f;

    private Vector3 velocity;

    private void Awake()
    {
        Instance = this;

        // trava e esconde o cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }



    // === Movimento com WASD ===
    public void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal"); // A/D ou setas
        float v = Input.GetAxis("Vertical");   // W/S ou setas

        Vector3 move = transform.right * h + transform.forward * v;
        controller.Move(move * moveSpeed * Time.deltaTime);

        // gravidade
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // mantém "grudado" no chão
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // === Rotação com o mouse ===
    public void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // rotaciona o corpo no eixo Y
        transform.Rotate(Vector3.up * mouseX);

        // rotaciona a câmera no eixo X (para cima/baixo)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // evita girar 360°

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    // Funções de interface que você já tinha
    public void Move(float h, float v)
    {
        Vector3 move = transform.right * h + transform.forward * v;
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

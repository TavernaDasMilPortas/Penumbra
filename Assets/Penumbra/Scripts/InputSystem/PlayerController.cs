using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [Header("Refer�ncias")]
    public Camera playerCamera;          // Arraste a c�mera do jogador aqui
    public CharacterController controller; // Arraste o CharacterController do Player

    [Header("Configura��o Movimento")]
    public float moveSpeed = 2.5f;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;

    [Header("Configura��o Mouse")]
    public float mouseSensitivity = 500f;
    private float xRotation = 0f; // Controle vertical (olhar para cima/baixo)

    private Vector3 velocity;

    private void Awake()
    {
        Instance = this;

        // Trava e esconde o cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMovement();
        HandleMouseLook();

        // Libera cursor com ESC e trava de novo com clique
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // === Movimento com WASD ===
    public void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal"); // A/D
        float v = Input.GetAxis("Vertical");   // W/S

        Vector3 move = transform.right * h + transform.forward * v;
        controller.Move(move * moveSpeed * Time.deltaTime);

        // Gravidade
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // === Rota��o com o mouse ===
    public void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotaciona o corpo (horizontal)
        transform.Rotate(Vector3.up * mouseX);

        // Rota��o vertical da c�mera (limitada)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    // Fun��es extra (caso queira usar em outros scripts)
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

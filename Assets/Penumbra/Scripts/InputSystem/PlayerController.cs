using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [Header("Referências")]
    public Camera playerCamera;
    public CharacterController controller;

    [Header("Configuração Movimento")]
    public float moveSpeed = 2.5f;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;

    [Header("Configuração Mouse")]
    public float mouseSensitivity = 500f;
    private float xRotation = 0f;

    private Vector3 velocity;   // usada para gravidade e movimento adicional
    private Vector3 externalVelocity; // usada pelo StairLink

    private void Awake()
    {
        Instance = this;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // === Movimento com WASD ===
    public void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = transform.right * h + transform.forward * v;

        // movimento padrão + external velocity
        controller.Move((move * moveSpeed + externalVelocity) * Time.deltaTime);

        // gravidade
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        // external velocity se dissipa rápido
        externalVelocity = Vector3.Lerp(externalVelocity, Vector3.zero, Time.deltaTime * 8f);
    }

    // === Rotação com o mouse ===
    public void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    // === Utilitários para StairLink ===

    /// <summary>
    /// Retorna a velocidade do jogador no plano XZ.
    /// </summary>
    public Vector3 GetCurrentVelocity()
    {
        Vector3 flatVel = transform.forward * Input.GetAxis("Vertical") * moveSpeed +
                          transform.right * Input.GetAxis("Horizontal") * moveSpeed;
        return flatVel;
    }

    /// <summary>
    /// Define uma velocidade externa temporária (teleporte suave).
    /// </summary>
    public void SetExternalVelocity(Vector3 vel)
    {
        externalVelocity = vel;
    }


    // === Funções auxiliares ===

    public void Move(float h, float v)
    {
        Vector3 move = transform.right * h + transform.forward * v;
        controller.Move(move * moveSpeed * Time.deltaTime);
    }

    public void Stop()
    {
        velocity = Vector3.zero;
        externalVelocity = Vector3.zero;
    }

    public void Interagir()
    {
        Stop();
        InteractionHandler.Instance.nearestInteractable.Interact();
    }
}

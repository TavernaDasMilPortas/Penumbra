using UnityEngine;

public class LanternMotion : MonoBehaviour
{
    [Header("Referências")]
    public Transform targetSocket; // o ponto no personagem onde o lampião deveria estar
    public CharacterController playerController;

    [Header("Balanço ao andar")]
    public float walkBobAmplitude = 0.05f;
    public float walkBobFrequency = 8f;

    public float runBobAmplitude = 0.1f;
    public float runBobFrequency = 12f;

    [Header("Respiração (idle)")]
    public float breathingAmplitude = 0.02f;
    public float breathingFrequency = 1f;

    [Header("Suavização")]
    public float smoothPosition = 8f;
    public float smoothRotation = 8f;

    private Vector3 initialLocalPos;
    private Quaternion initialLocalRot;

    void Start()
    {
        initialLocalPos = transform.localPosition;
        initialLocalRot = transform.localRotation;

        if (playerController == null)
            playerController = FindAnyObjectByType<CharacterController>();
    }

    void LateUpdate()
    {
        // Atualizar posição base para seguir o socket
        transform.position = Vector3.Lerp(
            transform.position,
            targetSocket.position,
            Time.deltaTime * smoothPosition
        );

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            targetSocket.rotation,
            Time.deltaTime * smoothRotation
        );

        // ===== 1) Detectar intensidade do movimento =====
        float speed = playerController.velocity.magnitude;

        Vector3 offset = Vector3.zero;

        // ===== 2) Jogador PARADO → respiração =====
        if (speed < 0.1f)
        {
            float breath = Mathf.Sin(Time.time * breathingFrequency) * breathingAmplitude;
            offset += new Vector3(0, breath, 0);
        }
        else
        {
            // ===== 3) Jogador andando/correndo → balanço =====
            bool isRunning = speed > 3.5f;

            float amp = isRunning ? runBobAmplitude : walkBobAmplitude;
            float freq = isRunning ? runBobFrequency : walkBobFrequency;

            float bobX = Mathf.Sin(Time.time * freq) * amp;
            float bobY = Mathf.Cos(Time.time * freq * 0.5f) * amp * 0.5f;

            offset += new Vector3(bobX, bobY, 0);
        }

        // Aplicar o offset local final
        transform.position += transform.right * offset.x;
        transform.position += transform.up * offset.y;
    }
}

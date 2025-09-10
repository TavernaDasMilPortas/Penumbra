using UnityEngine;

public class FacingHighlighter : MonoBehaviour
{
    [Header("Configuração do Facing")]
    public Transform facingOrigin;   // geralmente sua CameraPlayer
    public float viewAngle = 45f;    // ângulo do cone de visão
    public float viewDistance = 5f;  // alcance da visão

    [Header("Configuração do Objeto")]
    public Renderer targetRenderer;  // renderer do objeto que será alterado
    public Color highlightColor = Color.red;
    private Color originalColor;

    private void Start()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponent<Renderer>();

        if (targetRenderer != null)
            originalColor = targetRenderer.material.color;
    }

    private void Update()
    {
        if (IsInView())
        {
            // dentro do campo de visão → muda a cor
            targetRenderer.material.color = highlightColor;
        }
        else
        {
            // fora do campo de visão → cor original
            targetRenderer.material.color = originalColor;
        }
    }

    private bool IsInView()
    {
        if (facingOrigin == null || targetRenderer == null)
            return false;

        Vector3 dirToTarget = (transform.position - facingOrigin.position).normalized;

        // ângulo entre a frente da câmera e a direção do objeto
        float angle = Vector3.Angle(facingOrigin.forward, dirToTarget);

        if (angle > viewAngle) return false;

        // checa distância
        float distance = Vector3.Distance(facingOrigin.position, transform.position);
        if (distance > viewDistance) return false;

        // checa se tem algo bloqueando (Raycast)
        if (Physics.Raycast(facingOrigin.position, dirToTarget, out RaycastHit hit, viewDistance))
        {
            return hit.collider.gameObject == gameObject;
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        if (facingOrigin == null) return;

        Gizmos.color = Color.cyan;
        Vector3 left = Quaternion.Euler(0, -viewAngle, 0) * facingOrigin.forward;
        Vector3 right = Quaternion.Euler(0, viewAngle, 0) * facingOrigin.forward;

        Gizmos.DrawLine(facingOrigin.position, facingOrigin.position + left * viewDistance);
        Gizmos.DrawLine(facingOrigin.position, facingOrigin.position + right * viewDistance);
    }
}

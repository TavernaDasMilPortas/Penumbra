using UnityEngine;

public class FloatingLanternController : MonoBehaviour
{
    [Header("Referência do Lampião")]
    public Transform holder;   // já posicionado na mão da câmera

    [Header("Anti-clip")]
    public float avoidRadius = 0.25f;
    public float avoidDistance = 1.0f;
    public LayerMask collisionMask;

    [Header("Suavização")]
    public float followSpeed = 10f;
    public float rotationSpeed = 12f;

    private Vector3 smoothVelocity;

    void Update()
    {
        if (holder == null) return;

        // posição ideal = exatamente o holder
        Vector3 targetPos = holder.position;

        // direção do holder até o lampião ideal
        Vector3 dir = (targetPos - holder.position).normalized;
        float dist = Vector3.Distance(holder.position, targetPos);

        // ANTI-CLIPPING
        if (Physics.SphereCast(holder.position, avoidRadius, dir, out RaycastHit hit, dist, collisionMask))
        {
            targetPos = hit.point + hit.normal * avoidRadius;
        }

        // MOVIMENTO SUAVE
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPos,
            ref smoothVelocity,
            1f / followSpeed
        );

        // ROTAÇÃO SUAVE
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            holder.rotation,
            Time.deltaTime * rotationSpeed
        );
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, avoidRadius);
    }
}

using UnityEngine;

public class InteractionHandler : MonoBehaviour
{
    public static InteractionHandler Instance { get; private set; }

    [Header("Configurações Raycast")]
    public float interactionDistance = 5f; // raio do raycast
    public LayerMask interactionLayer;

    [Header("Configurações Overlap")]
    public float overlapRadius = 2f; // raio do OverlapSphere
    public LayerMask overlapLayer;

    [Header("Status Atual")]
    public IInteractable nearestInteractable;
    [HideInInspector] public IInteractable lastHighlighted;

    public Camera mainCamera;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Raycast central da câmera para interação
    /// </summary>
    public void FindInteractable()
    {
        IInteractable raycastTarget = GetInteractableByRaycast();
        UpdateHighlight(raycastTarget);
    }

    /// <summary>
    /// Retorna o IInteractable detectado pelo raycast ou null
    /// </summary>
    public IInteractable GetInteractableByRaycast()
    {
        if (mainCamera == null) return null;

        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance, interactionLayer))
        {
            return hit.collider.GetComponent<IInteractable>();
        }

        return null;
    }

    /// <summary>
    /// Função separada para detectar o mais próximo usando OverlapSphere
    /// </summary>
    public IInteractable GetNearestByOverlap()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, overlapRadius, overlapLayer);

        IInteractable closest = null;
        float shortestDistance = float.MaxValue;

        foreach (Collider hit in hits)
        {
            IInteractable interactable = hit.GetComponent<IInteractable>();
            if (interactable == null) continue;

            float distance = Vector3.Distance(transform.position, hit.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                closest = interactable;
            }
        }

        return closest;
    }

    private void UpdateHighlight(IInteractable newInteractable)
    {
        nearestInteractable = newInteractable;

        //OutlineManager.Instance?.Highlight(newInteractable);

        lastHighlighted = newInteractable;
    }

    private void OnDrawGizmosSelected()
    {
        if (mainCamera != null)
        {
            Gizmos.color = Color.red;

            // origem e direção do Raycast
            Vector3 start = mainCamera.transform.position;
            Vector3 end = start + mainCamera.transform.forward * interactionDistance;

            // desenha apenas a linha do Raycast
            Gizmos.DrawLine(start, end);
        }

        // opcional: visualize o OverlapSphere
        //Gizmos.color = Color.yellow;
        //Gizmos.DrawWireSphere(transform.position, overlapRadius);
    }


    // Função para desenhar “cilindro” como gizmo (linha grossa)

}

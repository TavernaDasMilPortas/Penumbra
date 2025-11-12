using UnityEngine;
using System.Collections.Generic;

public class InteractionHandler : MonoBehaviour
{
    public static InteractionHandler Instance { get; private set; }

    [Header("Configurações Raycast")]
    public float interactionDistance = 5f;
    public LayerMask interactionLayer;

    [Header("Configurações Overlap")]
    public float overlapRadius = 2f;
    public LayerMask overlapLayer;

    [Header("Status Atual")]
    public IInteractable nearestInteractable;
    [HideInInspector] public IInteractable lastHighlighted;

    public Camera mainCamera;

    // 🔹 Armazena a layer original dos objetos destacados
    private readonly Dictionary<GameObject, int> originalLayers = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void FindInteractable()
    {
        IInteractable raycastTarget = GetInteractableByRaycast();
        UpdateHighlight(raycastTarget);

        if (raycastTarget != null)
        {
            ActionHintManager.Instance.ShowHint("E", "Interagir", priority: 10);
        }
        else
        {
            ActionHintManager.Instance.HideHint("E");
        }
    }

    public IInteractable GetInteractableByRaycast()
    {
        if (mainCamera == null) return null;

        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance, interactionLayer))
        {
            var col = hit.collider;
            if (col == null) return null;

            if (col.TryGetComponent<IInteractable>(out IInteractable interactable))
            {
                try
                {
                    if (interactable != null && interactable.IsInteractable)
                        return interactable;
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning($"Interagível encontrado, mas falha ao acessar IsInteractable: {ex.Message}");
                }
            }
        }

        return null;
    }

    private void UpdateHighlight(IInteractable newInteractable)
    {
        // 🔸 Verifica se o último interagível ainda existe
        if (lastHighlighted != null)
        {
            var lastMB = lastHighlighted as MonoBehaviour;
            if (lastMB == null || lastMB.gameObject == null)
            {
                // O objeto foi destruído, então limpamos a referência
                lastHighlighted = null;
            }
        }

        // 🔸 Evita processar se for o mesmo interagível ainda ativo
        if (newInteractable == lastHighlighted)
            return;

        // 🔸 Remove o destaque do anterior (se ainda existe)
        if (lastHighlighted != null)
        {
            var lastMB = lastHighlighted as MonoBehaviour;
            if (lastMB != null && lastMB.gameObject != null)
                RestoreOriginalLayer(lastMB.gameObject);
        }

        // 🔸 Aplica destaque no novo objeto (se válido e não destruído)
        if (newInteractable != null)
        {
            var newMB = newInteractable as MonoBehaviour;
            if (newMB != null && newMB.gameObject != null)
                ApplyOutlineLayer(newMB.gameObject);
        }

        lastHighlighted = newInteractable;
        nearestInteractable = newInteractable;
    }


    /// <summary>
    /// Coloca o objeto (e filhos) na layer "OutlineObject" e salva as layers originais.
    /// </summary>
    private void ApplyOutlineLayer(GameObject obj)
    {
        int outlineLayer = LayerMask.NameToLayer("OutlineObject");
        if (outlineLayer == -1)
        {
            Debug.LogWarning("Layer 'OutlineObject' não existe! Crie-a no Project Settings > Tags and Layers.");
            return;
        }

        if (!originalLayers.ContainsKey(obj))
            SaveOriginalLayers(obj);

        SetLayerRecursively(obj, outlineLayer);
    }

    /// <summary>
    /// Restaura as layers originais salvas quando o objeto deixa de ser destacado.
    /// </summary>
    private void RestoreOriginalLayer(GameObject obj)
    {
        if (obj == null) return;

        if (originalLayers.TryGetValue(obj, out int originalLayer))
        {
            SetLayerRecursively(obj, originalLayer);
            originalLayers.Remove(obj);
        }
    }

    /// <summary>
    /// Salva a layer atual (antes do destaque).
    /// </summary>
    private void SaveOriginalLayers(GameObject obj)
    {
        if (obj == null) return;
        originalLayers[obj] = obj.layer;
    }

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
            SetLayerRecursively(child.gameObject, layer);
    }

    private void OnDrawGizmosSelected()
    {
        if (mainCamera != null)
        {
            Gizmos.color = Color.gray;
            Vector3 start = mainCamera.transform.position;
            Vector3 end = start + mainCamera.transform.forward * interactionDistance;
            Gizmos.DrawLine(start, end);
        }
    }

    public void Refresh()
    {
        FindInteractable();
    }
}

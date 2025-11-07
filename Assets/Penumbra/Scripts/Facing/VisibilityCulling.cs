using UnityEngine;
using System.Collections.Generic;

public class VisibilityCulling : MonoBehaviour
{
    public static VisibilityCulling Instance { get; private set; }

    [Header("Referências")]
    public FacingSystem facingSystem;
    public Transform player;

    [Header("Configurações")]
    [Tooltip("Se true, os objetos são completamente desativados. Caso contrário, apenas os Renderers são desativados.")]
    public bool disableCompletely = true;

    [Header("Layers Especiais")]
    public LayerMask floorLayer;
    public float floorSafeRadius = 2f;
    public LayerMask essentialLayers;
    [Tooltip("Objetos nessas layers serão completamente ignorados pelo sistema.")]
    public LayerMask ignoredLayers;

    // Lista de todos os objetos que podem ser culled
    private List<Collider> allCulledObjects = new List<Collider>();
    private HashSet<Collider> currentlyActive = new HashSet<Collider>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        if (player == null && Camera.main != null)
            player = Camera.main.transform;

        if (facingSystem == null)
            facingSystem = FindObjectOfType<FacingSystem>();

        RegisterCulledObjects();

        foreach (var col in allCulledObjects)
            SetObjectActive(col, false);

        facingSystem.OnEnterVision.AddListener(ActivateObject);
        facingSystem.OnExitVision.AddListener(DeactivateObject);
    }

    private void RegisterCulledObjects()
    {
        allCulledObjects.Clear();

        foreach (var col in FindObjectsOfType<Collider>())
        {
            int objLayer = col.gameObject.layer;

            // Ignora layers não relevantes
            if (((1 << objLayer) & ignoredLayers) != 0)
                continue;

            // Apenas colliders que estão nas layers alvo do FacingSystem
            if (((1 << objLayer) & facingSystem.targetMask) != 0)
            {
                allCulledObjects.Add(col);
            }
        }
    }

    /// <summary>
    /// Registra dinamicamente um novo objeto no sistema de culling.
    /// </summary>
    public void Register(Collider col)
    {
        if (col == null) return;

        int objLayer = col.gameObject.layer;

        // Ignora layers não relevantes
        if (((1 << objLayer) & ignoredLayers) != 0)
            return;

        if (allCulledObjects.Contains(col))
            return;

        if (((1 << objLayer) & facingSystem.targetMask) != 0)
        {
            allCulledObjects.Add(col);
            SetObjectActive(col, false);
        }
    }

    private void ActivateObject(Collider col)
    {
        if (col == null) return;

        if (ShouldIgnore(col)) return;

        if (!currentlyActive.Contains(col))
        {
            SetObjectActive(col, true);
            currentlyActive.Add(col);
        }
    }

    private void DeactivateObject(Collider col)
    {
        if (col == null) return;

        if (ShouldIgnore(col)) return;

        if (currentlyActive.Contains(col))
        {
            SetObjectActive(col, false);
            currentlyActive.Remove(col);
        }
    }

    private bool ShouldIgnore(Collider col)
    {
        int layer = col.gameObject.layer;
        return IsFloorAndPlayerOnTop(col) || IsEssential(col) || IsIgnored(col);
    }

    private void SetObjectActive(Collider col, bool state)
    {
        if (disableCompletely)
        {
            col.gameObject.SetActive(state);
        }
        else
        {
            Renderer[] renderers = col.GetComponentsInChildren<Renderer>(true);
            foreach (var r in renderers)
                r.enabled = state;
        }
    }

    private bool IsFloorAndPlayerOnTop(Collider col)
    {
        if (((1 << col.gameObject.layer) & floorLayer) == 0) return false;

        Vector3 playerPos = new Vector3(player.position.x, 0f, player.position.z);
        Vector3 floorPos = new Vector3(col.transform.position.x, 0f, col.transform.position.z);
        return Vector3.Distance(playerPos, floorPos) <= floorSafeRadius;
    }

    private bool IsEssential(Collider col)
    {
        return ((1 << col.gameObject.layer) & essentialLayers) != 0;
    }

    private bool IsIgnored(Collider col)
    {
        return ((1 << col.gameObject.layer) & ignoredLayers) != 0;
    }
}

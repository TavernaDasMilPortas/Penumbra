using UnityEngine;
using System.Collections.Generic;

public class VisibilityCulling : MonoBehaviour
{
    [Header("Referências")]
    public FacingSystem facingSystem;
    public Transform player;

    [Header("Configurações")]
    public bool disableCompletely = true;

    [Header("Layers especiais")]
    public LayerMask floorLayer;
    public float floorSafeRadius = 2f;
    public LayerMask essentialLayers;

    // Lista de todos os objetos que podem ser culled
    private List<Collider> allCulledObjects = new List<Collider>();
    private HashSet<Collider> currentlyActive = new HashSet<Collider>();

    private void Awake()
    {
        if (player == null && Camera.main != null)
            player = Camera.main.transform;

        if (facingSystem == null)
            facingSystem = FindObjectOfType<FacingSystem>();

        // Pré-cadastra apenas os objetos que estão nas layers target
        RegisterCulledObjects();

        // Inicializa todos os objetos como invisíveis
        foreach (var col in allCulledObjects)
            SetObjectActive(col, false);

        // Assina os eventos do FacingSystem
        facingSystem.OnEnterVision.AddListener(ActivateObject);
        facingSystem.OnExitVision.AddListener(DeactivateObject);
    }

    private void RegisterCulledObjects()
    {
        allCulledObjects.Clear();

        foreach (var col in FindObjectsOfType<Collider>())
        {
            // Apenas colliders que estão nas layers alvo
            if (((1 << col.gameObject.layer) & facingSystem.targetMask) != 0)
            {
                allCulledObjects.Add(col);
            }
        }
    }

    private void ActivateObject(Collider col)
    {
        if (col == null) return;

        if (IsFloorAndPlayerOnTop(col)) return;
        if (IsEssential(col)) return;

        if (!currentlyActive.Contains(col))
        {
            SetObjectActive(col, true);
            currentlyActive.Add(col);
        }
    }

    private void DeactivateObject(Collider col)
    {
        if (col == null) return;

        if (IsFloorAndPlayerOnTop(col)) return;
        if (IsEssential(col)) return;

        if (currentlyActive.Contains(col))
        {
            SetObjectActive(col, false);
            currentlyActive.Remove(col);
        }
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
}

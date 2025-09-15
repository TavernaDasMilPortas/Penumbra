using UnityEngine;
using System.Collections.Generic;

public class VisibilityCulling : MonoBehaviour
{
    public static VisibilityCulling Instance { get; private set; }

    [Header("Refer�ncias")]
    public FacingSystem facingSystem;
    public Transform player;

    [Header("Configura��es")]
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
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (player == null && Camera.main != null)
            player = Camera.main.transform;

        if (facingSystem == null)
            facingSystem = FindObjectOfType<FacingSystem>();

        // Pr�-cadastra apenas os objetos que est�o nas layers target
        RegisterCulledObjects();

        // Inicializa todos os objetos como invis�veis
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
            // Apenas colliders que est�o nas layers alvo
            if (((1 << col.gameObject.layer) & facingSystem.targetMask) != 0)
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

        // Se j� estiver registrado, ignora
        if (allCulledObjects.Contains(col)) return;

        // S� adiciona se estiver na layer de interesse
        if (((1 << col.gameObject.layer) & facingSystem.targetMask) != 0)
        {
            allCulledObjects.Add(col);

            // Garante que comece invis�vel
            SetObjectActive(col, false);
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


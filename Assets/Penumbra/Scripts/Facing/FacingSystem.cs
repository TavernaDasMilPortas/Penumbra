using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class FacingSystem : MonoBehaviour
{
    [Header("Configuração de visão")]
    public float viewDistance = 15f;
    public LayerMask targetMask;

    [Header("Debug")]
    public bool showGizmos = true;

    public static FacingSystem Instance { get; private set; }
    private Camera mainCamera;
    private Plane[] frustumPlanes;

    // Objetos atualmente visíveis
    private readonly HashSet<Collider> currentlyVisible = new HashSet<Collider>();

    // Eventos globais
    public UnityEvent<Collider> OnEnterVision;
    public UnityEvent<Collider> OnExitVision;

    private void Awake()
    {
        mainCamera = Camera.main;
        if (OnEnterVision == null) OnEnterVision = new UnityEvent<Collider>();
        if (OnExitVision == null) OnExitVision = new UnityEvent<Collider>();
    }

    private void Update()
    {
        if (GameStateManager.Instance.CurrentState == InputState.Gameplay)
        {
            UpdateVisibleTargets();
        }

    }

    public void UpdateVisibleTargets()
    {
        HashSet<Collider> newVisible = new HashSet<Collider>();

        // Calcula os planos do frustum da câmera
        frustumPlanes = GeometryUtility.CalculateFrustumPlanes(mainCamera);

        // Pega candidatos dentro do raio
        Collider[] hits = Physics.OverlapSphere(
            mainCamera.transform.position,
            viewDistance,
            targetMask,
            QueryTriggerInteraction.Collide
        );


        foreach (var hit in hits)
        {
            if (GeometryUtility.TestPlanesAABB(frustumPlanes, hit.bounds))
            {
                newVisible.Add(hit);

                // Entrou na visão
                if (!currentlyVisible.Contains(hit))
                {
                    OnEnterVision.Invoke(hit);
                }
            }
        }

        // Saiu da visão
        foreach (var col in currentlyVisible)
        {
            if (!newVisible.Contains(col))
            {
                OnExitVision.Invoke(col);
            }
        }

        // Atualiza estado
        currentlyVisible.Clear();
        foreach (var col in newVisible) currentlyVisible.Add(col);
    }

    private void OnDrawGizmosSelected()
    {
        if (!showGizmos || currentlyVisible == null) return;

        Gizmos.color = Color.green;
        foreach (var col in currentlyVisible)
        {
            if (col != null)
                Gizmos.DrawLine(mainCamera.transform.position, col.transform.position);
        }
    }


    public bool IsCurrentlyVisible(Collider col)
    {
        return currentlyVisible.Contains(col);
    }
}

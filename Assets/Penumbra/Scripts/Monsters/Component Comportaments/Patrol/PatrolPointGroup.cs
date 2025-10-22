using UnityEngine;
using System.Collections.Generic;

public class PatrolPointGroup : MonoBehaviour
{
    [Header("Configura��o")]
    [Tooltip("PointReference que define o objeto base que cont�m os pontos de patrulha como filhos.")]
    public PointReference basePoint;

    [Tooltip("Lista autom�tica dos pontos de patrulha (preenchida a partir dos filhos do Point).")]
    public Transform[] patrolPoints;

    public string patrolGroupName;

    [Header("Debug")]
    public Color gizmoColor = Color.cyan;
    public float gizmoRadius = 0.3f;
    public bool autoUpdateInEditor = true;

    private void OnValidate()
    {
        if (autoUpdateInEditor)
            RefreshPatrolPoints();
    }

    /// <summary>
    /// Atualiza a lista de patrolPoints com base nos filhos do objeto referenciado pelo PointReference.
    /// </summary>
    public void RefreshPatrolPoints()
    {
        patrolPoints = new Transform[0];

        if (basePoint == null || string.IsNullOrEmpty(basePoint.pointName))
            return;

        if (PointManager.Instance == null)
        {
            Debug.LogWarning("[PatrolPointGroup] PointManager n�o encontrado na cena!");
            return;
        }

        Point basePointObj = PointManager.Instance.GetPointByName(basePoint.pointName);
        if (basePointObj == null)
        {
            Debug.LogWarning($"[PatrolPointGroup] Nenhum Point encontrado com o nome '{basePoint.pointName}'");
            return;
        }

        // Coleta todos os filhos do transform do Point
        List<Transform> points = new List<Transform>();
        foreach (Transform child in basePointObj.selfTransform)
        {
            if (child != null)
                points.Add(child);
        }

        patrolPoints = points.ToArray();
    }

    private void OnDrawGizmos()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            if (autoUpdateInEditor)
                RefreshPatrolPoints();
            else
                return;
        }

        Gizmos.color = gizmoColor;

        foreach (var p in patrolPoints)
        {
            if (p == null) continue;
            Gizmos.DrawSphere(p.position, gizmoRadius);
        }

        // desenha linhas entre os pontos
        for (int i = 0; i < patrolPoints.Length - 1; i++)
        {
            if (patrolPoints[i] != null && patrolPoints[i + 1] != null)
            {
                Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[i + 1].position);
            }
        }
    }
}

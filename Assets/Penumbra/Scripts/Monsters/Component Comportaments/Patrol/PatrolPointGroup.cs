using UnityEngine;
using System.Collections.Generic;

public class PatrolPointGroup : MonoBehaviour
{
    [Header("Configuração")]
    [Tooltip("Objeto que contém os pontos de patrulha como filhos.")]
    public Transform baseGroup;

    [Tooltip("Lista automática dos pontos de patrulha (preenchida a partir dos filhos de baseGroup).")]
    public Transform[] patrolPoints;
    public string patrolGroupName;

    [Header("Debug")]
    public Color gizmoColor = Color.cyan;
    public float gizmoRadius = 0.3f;
    public bool autoUpdateInEditor = true;

    private void OnValidate()
    {
        if (baseGroup != null && autoUpdateInEditor)
            RefreshPatrolPoints();
    }

    /// <summary>
    /// Atualiza a lista de patrolPoints com base nos filhos do baseGroup.
    /// </summary>
    public void RefreshPatrolPoints()
    {
        if (baseGroup == null)
        {
            patrolPoints = new Transform[0];
            return;
        }

        List<Transform> points = new List<Transform>();
        foreach (Transform child in baseGroup)
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
            if (baseGroup != null)
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

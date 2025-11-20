using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PointGroup", menuName = "Game/Point Group")]
public class PointGroup : ScriptableObject
{
    public string groupName;

    [Tooltip("Lista de PointReferences usados por este grupo.")]
    public List<PointReference> points = new List<PointReference>();
}
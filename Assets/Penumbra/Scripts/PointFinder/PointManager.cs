using UnityEngine;
using System.Collections.Generic;

public class PointManager : MonoBehaviour
{
    public static PointManager Instance { get; private set; }
    public static List<string> AllPointNames = new List<string>();

    [Header("📍 Pontos encontrados")]
    public List<Point> points = new List<Point>();

    private Dictionary<string, Point> pointLookup = new Dictionary<string, Point>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        RefreshPoints();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!Application.isPlaying)
            RefreshPointsEditor();
    }
#endif

    /// <summary>
    /// Atualiza os pontos no modo Play.
    /// </summary>
    public void RefreshPoints()
    {
        points.Clear();
        pointLookup.Clear();
        AllPointNames.Clear();

        Point[] foundPoints = FindObjectsOfType<Point>(true);
        foreach (var point in foundPoints)
        {
            points.Add(point);
            if (!pointLookup.ContainsKey(point.objectName))
                pointLookup[point.objectName] = point;

            if (!AllPointNames.Contains(point.objectName))
                AllPointNames.Add(point.objectName);
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// Atualiza os pontos no Editor (inclui inativos e objetos fora do modo Play).
    /// </summary>
    public void RefreshPointsEditor()
    {
        points.Clear();
        pointLookup.Clear();
        AllPointNames.Clear();

        var foundPoints = UnityEngine.Resources.FindObjectsOfTypeAll<Point>();
        foreach (var p in foundPoints)
        {
            if (UnityEditor.EditorUtility.IsPersistent(p.gameObject)) continue;
            if (p.gameObject.hideFlags != HideFlags.None) continue;

            points.Add(p);
            if (!pointLookup.ContainsKey(p.objectName))
                pointLookup[p.objectName] = p;

            if (!AllPointNames.Contains(p.objectName))
                AllPointNames.Add(p.objectName);
        }

        // Garante atualização no Inspector
        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif

    public List<Point> GetPointsByName(List<string> names)
    {
        List<Point> result = new List<Point>();
        foreach (var name in names)
        {
            if (pointLookup.TryGetValue(name, out Point p))
                result.Add(p);
            else
                Debug.LogWarning($"❌ Point '{name}' não encontrado no PointManager.");
        }
        return result;
    }

    public Point GetPointByName(string name)
    {
        if (pointLookup.TryGetValue(name, out Point p))
            return p;

        Debug.LogWarning($"❌ Nenhum Point encontrado com o nome: {name}");
        return null;
    }
}

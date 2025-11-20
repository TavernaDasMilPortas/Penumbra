using UnityEngine;
using System.Collections.Generic;

public class PointManager : MonoBehaviour
{
    public static PointManager Instance { get; private set; }

    public static List<string> AllPointNames = new List<string>();

    [Header("📍 Pontos encontrados")]
    public List<Point> points = new List<Point>();

    [Header("📂 Grupos encontrados")]
    public List<PointGroup> groups = new List<PointGroup>();

    private Dictionary<string, Point> pointLookup = new Dictionary<string, Point>();
    private Dictionary<string, PointGroup> groupLookup = new Dictionary<string, PointGroup>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        RefreshPoints();
        RefreshGroups();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            RefreshPointsEditor();
            RefreshGroupsEditor();
        }
    }
#endif

    // =====================================================================
    // 🟦 1) PONTOS (play mode)
    // =====================================================================
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
    // =====================================================================
    // 🟩 2) PONTOS (editor)
    // =====================================================================
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

        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif

    // =====================================================================
    // 🟦 3) GRUPOS (play mode)
    // =====================================================================
    public void RefreshGroups()
    {
        groups.Clear();
        groupLookup.Clear();

        // Encontrar todos PointGroup carregados na cena ou no Resources
        PointGroup[] foundGroups = Resources.FindObjectsOfTypeAll<PointGroup>();

        foreach (var g in foundGroups)
        {
            if (!groupLookup.ContainsKey(g.groupName))
                groupLookup[g.groupName] = g;

            if (!groups.Contains(g))
                groups.Add(g);
        }
    }

#if UNITY_EDITOR
    // =====================================================================
    // 🟩 4) GRUPOS (editor)
    // =====================================================================
    public void RefreshGroupsEditor()
    {
        groups.Clear();
        groupLookup.Clear();

        var foundGroups = Resources.FindObjectsOfTypeAll<PointGroup>();

        foreach (var g in foundGroups)
        {
            if (UnityEditor.EditorUtility.IsPersistent(g)) continue;

            if (!groupLookup.ContainsKey(g.groupName))
                groupLookup[g.groupName] = g;

            groups.Add(g);
        }

        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif

    // =====================================================================
    // 🔵 GETTERS ORIGINAIS (totalmente preservados)
    // =====================================================================
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

    // =====================================================================
    // 🟣 NOVOS MÉTODOS DE GRUPOS
    // =====================================================================

    public PointGroup GetGroup(string name)
    {
        if (groupLookup.TryGetValue(name, out PointGroup g))
            return g;

        Debug.LogWarning($"❌ PointGroup '{name}' não encontrado.");
        return null;
    }

    public Point GetRandomPointFromGroup(string name)
    {
        var group = GetGroup(name);
        if (group == null)
        {
            Debug.LogWarning($"❌ Grupo '{name}' não existe.");
            return null;
        }

        if (group.points == null || group.points.Count == 0)
        {
            Debug.LogWarning($"⚠ Grupo '{name}' não possui PointReference nenhum.");
            return null;
        }

        // Resolver todos os PointReference para Points reais na cena
        List<Point> resolved = new List<Point>();

        foreach (var pref in group.points)
        {
            if (pref == null || string.IsNullOrEmpty(pref.pointName)) continue;

            Point p = GetPointByName(pref.pointName);

            if (p != null)
                resolved.Add(p);
        }

        if (resolved.Count == 0)
        {
            Debug.LogWarning($"⚠ Nenhum Point válido encontrado no grupo '{name}'.");
            return null;
        }

        return resolved[Random.Range(0, resolved.Count)];
    }
    public List<Point> GetPointsFromGroup(string groupName)
    {
        var group = GetGroup(groupName);
        if (group == null || group.points == null)
            return new List<Point>();

        List<Point> resolved = new List<Point>();

        foreach (var pref in group.points)
        {
            if (pref == null || string.IsNullOrEmpty(pref.pointName))
                continue;

            Point p = GetPointByName(pref.pointName);
            if (p != null)
                resolved.Add(p);
        }

        return resolved;
    }

}

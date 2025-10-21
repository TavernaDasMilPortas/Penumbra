using System.Collections.Generic;
using UnityEngine;

public class PointManager : MonoBehaviour
{
    public static PointManager Instance { get; private set; }

    private List<Point> allPoints = new List<Point>();
    private Dictionary<string, Point> pointLookup = new Dictionary<string, Point>();

    private void Awake()
    {
        // Garante que exista apenas uma instância
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Carrega todos os points automaticamente
        LoadAllPoints();
    }

    private void LoadAllPoints()
    {
        allPoints.Clear();
        pointLookup.Clear();

        Point[] foundPoints = FindObjectsOfType<Point>();

        foreach (var p in foundPoints)
        {
            allPoints.Add(p);

            if (!string.IsNullOrEmpty(p.objectName))
            {
                if (!pointLookup.ContainsKey(p.objectName))
                    pointLookup.Add(p.objectName, p);
                else
                    Debug.LogWarning($"⚠️ Point duplicado com nome: {p.objectName}");
            }
        }

        Debug.Log($"✅ {allPoints.Count} Points carregados.");
    }

    public Point GetPointByName(string name)
    {
        if (pointLookup.TryGetValue(name, out Point point))
            return point;

        Debug.LogWarning($"❌ Nenhum Point encontrado com o nome: {name}");
        return null;
    }

    public List<Point> GetPointsByName(string[] names)
    {
        List<Point> foundPoints = new List<Point>();

        foreach (string name in names)
        {
            if (pointLookup.TryGetValue(name, out Point point))
            {
                foundPoints.Add(point);
            }
            else
            {
                Debug.LogWarning($"❌ Nenhum Point encontrado com o nome: {name}");
            }
        }

        return foundPoints;
    }


    public List<Point> GetAllPoints()
    {
        return allPoints;
    }
}

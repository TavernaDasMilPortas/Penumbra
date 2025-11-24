using UnityEngine;
using System.Collections.Generic;

public class AutoItemSpawner : MonoBehaviour
{
    public Transform spawnContainer;

    private void Start()
    {
        // Spawn padrão
        SpawnAllPoints();
    }

    // ============================================================
    // 1️⃣ SPAWN NORMAL → TODOS OS POINTS COM spawnItem
    // ============================================================
    public void SpawnAllPoints() 
    {
        foreach (var p in PointManager.Instance.points)
        {
            if (p.spawnItem == null) continue;
            SpawnAtPoint(p, p.spawnItem);
        }
    }

    // ============================================================
    // 2️⃣ SPAWN DE MÚLTIPLOS ITENS EM UM POINTGROUP (SEM REPETIÇÃO)
    // ============================================================
    public void SpawnGroupItems(string groupName, List<Item> itemsToSpawn)
    {
        var points = PointManager.Instance.GetPointsFromGroup(groupName);

        if (points == null || points.Count == 0)
        {
            Debug.LogError($"[AutoItemSpawner] Grupo '{groupName}' está vazio.");
            return;
        }

        // Filtra points que ainda NÃO possuem instâncias
        List<Point> freePoints = new();
        foreach (var p in points)
        {
            if (p.selfTransform.childCount == 0) // ainda não spawnou nada
                freePoints.Add(p);
        }

        if (freePoints.Count == 0)
        {
            Debug.LogWarning($"[AutoItemSpawner] Todos os points do grupo '{groupName}' já estão ocupados.");
            return;
        }

        // Embaralha os points livres
        Shuffle(freePoints);

        int max = Mathf.Min(itemsToSpawn.Count, freePoints.Count);

        for (int i = 0; i < max; i++)
        {
            SpawnAtPoint(freePoints[i], itemsToSpawn[i]);
        }
    }

    // ============================================================
    // 3️⃣ FUNÇÃO INTERNA → SPAWN NUM PONTO
    // ============================================================
    private void SpawnAtPoint(Point p, Item item)
    {
        if (item == null || item.handPrefab == null)
            return;

        GameObject instance = Instantiate(item.handPrefab);

        Transform parent = spawnContainer != null ? spawnContainer : p.selfTransform;
        instance.transform.SetParent(parent, false);

        ItemAlignmentUtility.ApplyAlignment(instance, parent, item);

        var inst = instance.GetComponent<ItemInstance>();
        if (inst == null) inst = instance.AddComponent<ItemInstance>();

        inst.data = item;
        inst.originPoint = p;

        Debug.Log($"[AutoItemSpawner] Spawnou '{item.itemName}' em '{p.objectName}'.");
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int r = Random.Range(i, list.Count);
            (list[i], list[r]) = (list[r], list[i]);
        }
    }
}

using UnityEngine;
using System.Collections.Generic;

public class AutoItemSpawner : MonoBehaviour
{
    public Transform spawnContainer;

    private void Start()
    {
        SpawnAllPoints();
    }

    // ============================================================
    // 1️⃣ SPAWN NORMAL → COM VERIFICAÇÃO DE NIGHTDATA
    // ============================================================
    public void SpawnAllPoints()
    {
        NightData currentNight = NightManager.Instance?.CurrentNight;

        foreach (var p in PointManager.Instance.points)
        {
            if (p.spawnItem == null) continue;

            // --- NOVA REGRA ---
            if (p.requiredNight != null && p.requiredNight != currentNight)
                continue;

            SpawnAtPoint(p, p.spawnItem);
        }
    }

    // ============================================================
    // 2️⃣ SPAWN DE MÚLTIPLOS ITENS EM UM POINTGROUP
    // ============================================================
    public void SpawnGroupItems(string groupName, List<Item> itemsToSpawn)
    {
        var points = PointManager.Instance.GetPointsFromGroup(groupName);
        NightData currentNight = NightManager.Instance?.CurrentNight;

        if (points == null || points.Count == 0)
        {
            Debug.LogError($"[AutoItemSpawner] Grupo '{groupName}' está vazio.");
            return;
        }

        // Filtra para usar apenas points válidos nesta noite
        List<Point> validPoints = new();

        foreach (var p in points)
        {
            if (p.requiredNight == null || p.requiredNight == currentNight)
                if (p.selfTransform.childCount == 0) // não ocupados
                    validPoints.Add(p);
        }

        if (validPoints.Count == 0)
        {
            Debug.LogWarning($"[AutoItemSpawner] Nenhum point disponível para esta noite no grupo '{groupName}'.");
            return;
        }

        Shuffle(validPoints);

        int max = Mathf.Min(itemsToSpawn.Count, validPoints.Count);
        for (int i = 0; i < max; i++)
        {
            SpawnAtPoint(validPoints[i], itemsToSpawn[i]);
        }
    }

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

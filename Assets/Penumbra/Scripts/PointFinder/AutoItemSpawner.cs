using UnityEngine;
using System.Collections.Generic;

public class AutoItemSpawner : MonoBehaviour
{
    public Transform spawnContainer;

    private void Start()
    {
        SpawnAllPoints();
    }

    public void SpawnAllPoints()
    {
        Point[] points = FindObjectsOfType<Point>(true);

        foreach (var p in points)
        {
            if (p.spawnItem == null) continue;

            Item item = p.spawnItem;
            if (item.handPrefab == null)
            {
                continue;
            }

            GameObject instance = Instantiate(item.handPrefab);

            Transform parent = spawnContainer != null ? spawnContainer : p.selfTransform;

            // parent correto
            instance.transform.SetParent(parent, false);

            // alinhamento igual ao LeftArm
            ItemAlignmentUtility.ApplyAlignment(instance, parent, item);

            // Configurar ItemInstance
            ItemInstance inst = instance.GetComponent<ItemInstance>();
            if (inst == null) inst = instance.AddComponent<ItemInstance>();

            inst.data = item;
            inst.originPoint = p;

        }
    }
}

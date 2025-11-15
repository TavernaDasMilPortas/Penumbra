using UnityEngine;

[ExecuteAlways]
public class AutoItemSpawner : MonoBehaviour
{
    [Tooltip("Se marcado, os itens instanciados ficarão como filhos desse transform. Se nulo, serão filhos do Point.")]
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
                Debug.LogWarning($"[AutoItemSpawner] Point '{p.name}' tem spawnItem mas sem handPrefab (Item: {item.itemName}).");
                continue;
            }

            // Instancia o modelo físico
            GameObject go = Instantiate(
                item.handPrefab,
                p.selfTransform.position,
                p.selfTransform.rotation
            );

            go.name = item.handPrefab.name;

            // Define parent
            if (spawnContainer != null)
                go.transform.SetParent(spawnContainer, worldPositionStays: true);
            else
                go.transform.SetParent(p.selfTransform, worldPositionStays: true);

            // === APLICA TODOS OS OFFSETS DO ITEM ===

            // 1) Escala
            go.transform.localScale = item.placementScaleOffset;

            // 2) Rotação
            go.transform.localEulerAngles += item.placementRotationOffset;

            // 3) Posição
            go.transform.localPosition += item.placementOffset;

            // === CONFIGURA ITEMINSTANCE ===
            ItemInstance inst = go.GetComponent<ItemInstance>();
            if (inst == null) inst = go.AddComponent<ItemInstance>();

            inst.data = item;
            inst.originPoint = p;

            Debug.Log($"[AutoItemSpawner] Spawned '{go.name}' at Point '{p.name}' with offsets.");
        }
    }
}

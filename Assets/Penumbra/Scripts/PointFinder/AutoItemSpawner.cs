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
        // encontra todos os Points na cena
        Point[] points = FindObjectsOfType<Point>(true);

        foreach (var p in points)
        {
            if (p.spawnItem == null) continue;
            if (p.spawnItem.handPrefab == null)
            {
                Debug.LogWarning($"[AutoItemSpawner] Point '{p.name}' tem spawnItem mas sem handPrefab (Item: {p.spawnItem.itemName}).");
                continue;
            }

            // instancia o prefab físico
            GameObject go = Instantiate(p.spawnItem.handPrefab, p.selfTransform.position, p.selfTransform.rotation);

            // parenta no container (ou no próprio point)
            if (spawnContainer != null) go.transform.SetParent(spawnContainer, worldPositionStays: true);
            else go.transform.SetParent(p.selfTransform, worldPositionStays: true);

            // garante nome igual ao prefab (útil para lookup)
            go.name = p.spawnItem.handPrefab.name;

            // adiciona ou configura ItemInstance
            var inst = go.GetComponent<ItemInstance>();
            if (inst == null) inst = go.AddComponent<ItemInstance>();
            inst.data = p.spawnItem;
            inst.originPoint = p;

            // desativa por padrão (coleção necessária para o inventário)
            go.SetActive(true); // deixamos ativo inicialmente (por causa de cena), se desejar desativar ao spawn, mude para false

            Debug.Log($"[AutoItemSpawner] Spawned '{go.name}' at Point '{p.name}' for Item '{p.spawnItem.itemName}'");
        }
    }
}

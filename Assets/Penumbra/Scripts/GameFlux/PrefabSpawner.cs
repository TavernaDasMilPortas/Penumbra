using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PrefabSpawnData
{
    [Tooltip("Prefab a ser instanciado.")]
    public GameObject prefab;

    [Tooltip("Ponto onde o prefab será instanciado.")]
    public Point spawnPoint;
}

public class PrefabSpawner : MonoBehaviour
{
    [Header("Lista de Prefabs e Pontos de Spawn")]
    public List<PrefabSpawnData> spawnList = new List<PrefabSpawnData>();

    [Header("Configurações Opcionais")]
    public bool spawnOnStart = true;
    public bool parentToSpawner = false;

    private void Start()
    {
        if (spawnOnStart)
            SpawnAll();
    }

    /// <summary>
    /// Instancia todos os prefabs definidos na lista.
    /// </summary>
    public void SpawnAll()
    {
        foreach (var data in spawnList)
        {
            if (data.prefab == null || data.spawnPoint == null)
            {
                Debug.LogWarning($"[PrefabSpawner] Dados inválidos detectados em '{gameObject.name}'. Prefab ou Point ausente.");
                continue;
            }

            GameObject obj = Instantiate(data.prefab, data.spawnPoint.selfTransform.position, data.spawnPoint.selfTransform.rotation);

            if (parentToSpawner)
                obj.transform.SetParent(transform);

            // Opcional: Renomear o objeto instanciado
            obj.name = $"{data.prefab.name}_at_{data.spawnPoint.objectName}";

            Debug.Log($"[PrefabSpawner] Instanciado '{data.prefab.name}' em '{data.spawnPoint.objectName}'");
        }
    }

    /// <summary>
    /// Instancia um prefab específico pelo índice.
    /// </summary>
    public GameObject SpawnByIndex(int index)
    {
        if (index < 0 || index >= spawnList.Count)
        {
            Debug.LogWarning("[PrefabSpawner] Índice fora dos limites da lista!");
            return null;
        }

        var data = spawnList[index];
        if (data.prefab == null || data.spawnPoint == null)
        {
            Debug.LogWarning("[PrefabSpawner] Dados inválidos neste índice!");
            return null;
        }

        GameObject obj = Instantiate(data.prefab, data.spawnPoint.selfTransform.position, data.spawnPoint.selfTransform.rotation);
        if (parentToSpawner)
            obj.transform.SetParent(transform);

        obj.name = $"{data.prefab.name}_at_{data.spawnPoint.objectName}";
        return obj;
    }
}


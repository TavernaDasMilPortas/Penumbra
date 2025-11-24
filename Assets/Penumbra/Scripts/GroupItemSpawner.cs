using UnityEngine;
using System.Collections.Generic;

public class GroupItemSpawner : MonoBehaviour
{
    [Header("Spawner responsável por instanciar os objetos (auto detectado se vazio)")]
    public AutoItemSpawner spawner;

    [Header("Nome do grupo de points onde os itens serão spawnados")]
    public string groupName;

    [Header("Itens que serão spawnados (ordem não importa)")]
    public List<Item> itemsToSpawn = new();

    [Header("Spawn automático ao iniciar?")]
    public bool spawnOnStart = true;

    private void Awake()
    {
        // Se não foi definido manualmente, tentamos achar automaticamente
        if (spawner == null)
        {
            // 1️⃣ tenta no próprio objeto
            spawner = GetComponent<AutoItemSpawner>();

            // 2️⃣ tenta na cena inteira
            if (spawner == null)
                spawner = FindObjectOfType<AutoItemSpawner>();

            if (spawner != null)
                Debug.Log("[GroupItemSpawner] AutoItemSpawner encontrado automaticamente!");
            else
                Debug.LogError("[GroupItemSpawner] Nenhum AutoItemSpawner encontrado na cena!");
        }
    }

    private void Start()
    {
        if (spawnOnStart)
            SpawnItems();
    }

    [ContextMenu("Spawnar Itens Agora")]
    public void SpawnItems()
    {
        if (spawner == null)
        {
            Debug.LogError("[GroupItemSpawner] Nenhum AutoItemSpawner encontrado!");
            return;
        }

        if (string.IsNullOrWhiteSpace(groupName))
        {
            Debug.LogError("[GroupItemSpawner] groupName está vazio!");
            return;
        }

        if (itemsToSpawn == null || itemsToSpawn.Count == 0)
        {
            Debug.LogWarning("[GroupItemSpawner] Lista de itens está vazia, nada será spawnado.");
            return;
        }

        Debug.Log($"[GroupItemSpawner] Spawnando {itemsToSpawn.Count} itens no grupo '{groupName}'...");

        spawner.SpawnGroupItems(groupName, itemsToSpawn);
    }
}

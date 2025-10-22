using UnityEngine;
using System.Collections.Generic;


[System.Serializable]
public class SpawnData
{
    [Header("📦 Dados do Spawn")]
    public GameObject monsterPrefab;

    [Tooltip("Nomes dos pontos de spawn (selecionados via PointDrawer).")]
    public List<PointReference> spawnPointNames = new List<PointReference>();

    [HideInInspector]
    public List<Transform> resolvedSpawnPoints = new List<Transform>();

    public PatrolPointGroup patrolGroup;

    [Header("🕒 Evento de Tempo")]
    public TimerEventReference eventReference;
    [Range(1, 100)] public int choiceChance = 100;

    /// <summary>
    /// Resolve os nomes em Transforms reais através do PointManager.
    /// </summary>
    public void ResolvePoints()
    {
        resolvedSpawnPoints.Clear();

        if (PointManager.Instance == null)
        {
            Debug.LogWarning("[SpawnData] PointManager não encontrado na cena!");
            return;
        }

        // Cria uma lista de nomes a partir das referências
        List<string> names = new List<string>();
        foreach (var pr in spawnPointNames)
        {
            if (!string.IsNullOrEmpty(pr.pointName))
                names.Add(pr.pointName);
        }

        if (names.Count == 0)
        {
            Debug.LogWarning($"[SpawnData] Nenhum nome de ponto definido em SpawnData de {monsterPrefab?.name ?? "objeto indefinido"}.");
            return;
        }

        // Busca os pontos reais pelo nome
        var points = PointManager.Instance.GetPointsByName(names);
        foreach (var point in points)
        {
            if (point != null)
                resolvedSpawnPoints.Add(point.transform);
        }

        if (resolvedSpawnPoints.Count == 0)
        {
            Debug.LogWarning($"[SpawnData] Nenhum ponto válido encontrado para {monsterPrefab?.name ?? "Spawn sem nome"}.");
        }
    }
}


[System.Serializable]
public class SpawnEntry
{
    [Header("📦 Spawns")]
    public string spawnName = "Novo Spawn";
    public List<SpawnData> spawnData = new List<SpawnData>();

    [HideInInspector] public SpawnData chosenData; // armazenará o spawnData escolhido
}

public class TimerSpawnEvent : MonoBehaviour
{
    [Header("Configuração de Spawns")]
    public List<SpawnEntry> spawns = new List<SpawnEntry>();

    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (TimerEventScheduler.Instance == null)
        {
            Debug.LogError("[TimerSpawnEvent] TimerEventScheduler não encontrado na cena!");
            return;
        }

        foreach (var entry in spawns)
        {
            if (entry.spawnData == null || entry.spawnData.Count == 0)
            {
                Debug.LogWarning($"[TimerSpawnEvent] {entry.spawnName} não possui SpawnData configurado!");
                continue;
            }

            // 🔹 Escolhe um SpawnData com base na chance
            entry.chosenData = PickRandomSpawnData(entry.spawnData);

            if (entry.chosenData == null || entry.chosenData.eventReference == null)
            {
                Debug.LogWarning($"[TimerSpawnEvent] {entry.spawnName} não tem evento ou prefab válido!");
                continue;
            }

            // 🔹 Resolve os pontos de spawn (nome → Transform)
            entry.chosenData.ResolvePoints();

            int triggerSecond = entry.chosenData.eventReference.triggerSecond;
            Debug.Log($"[TimerSpawnEvent] Registrando evento '{entry.spawnName}' para {triggerSecond}s ({entry.chosenData.monsterPrefab?.name}).");

            var localEntry = entry;
            TimerEventScheduler.Instance.AddEvent(
                triggerSecond,
                () => TrySpawn(localEntry),
                $"Spawn '{localEntry.spawnName}'"
            );
        }
    }

    private SpawnData PickRandomSpawnData(List<SpawnData> dataList)
    {
        int totalChance = 0;
        foreach (var data in dataList)
            totalChance += data.choiceChance;

        int randomValue = Random.Range(0, totalChance);
        int cumulative = 0;

        foreach (var data in dataList)
        {
            cumulative += data.choiceChance;
            if (randomValue < cumulative)
                return data;
        }

        return dataList[0]; // fallback
    }

    private void TrySpawn(SpawnEntry entry)
    {
        if (entry.chosenData == null)
        {
            Debug.LogWarning($"[TimerSpawnEvent] Nenhum SpawnData escolhido para {entry.spawnName}");
            return;
        }

        var data = entry.chosenData;
        var prefabToSpawn = data.monsterPrefab;
        var spawnPoints = data.resolvedSpawnPoints;
        var patrolGroup = data.patrolGroup;

        if (player == null)
        {
            Debug.LogWarning("[TimerSpawnEvent] Player não encontrado!");
            return;
        }

        if (prefabToSpawn == null)
        {
            Debug.LogWarning($"[TimerSpawnEvent] {entry.spawnName} sem prefab definido!");
            return;
        }

        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            Debug.LogWarning($"[TimerSpawnEvent] {entry.spawnName} sem pontos de spawn válidos!");
            return;
        }

        List<Transform> validSpawns = new List<Transform>();

        foreach (var spawn in spawnPoints)
        {
            if (spawn == null) continue;

            RoomTracker tracker = spawn.GetComponent<RoomTracker>();
            if (tracker != null && tracker.CurrentRoom != null)
            {
                Room room = tracker.CurrentRoom;
                if (!room.Contains(player.position))
                    validSpawns.Add(spawn);
            }
            else
            {
                validSpawns.Add(spawn);
            }
        }

        if (validSpawns.Count == 0)
        {
            Debug.LogWarning($"[TimerSpawnEvent] Nenhum spawn válido encontrado para {entry.spawnName}");
            return;
        }

        Transform chosen = validSpawns[Random.Range(0, validSpawns.Count)];
        GameObject spawned = Instantiate(prefabToSpawn, chosen.position, chosen.rotation);

        Debug.Log($"[TimerSpawnEvent] Spawn '{entry.spawnName}' criado: {spawned.name} em {chosen.name}");

        if (patrolGroup != null)
        {
            Patrol patrol = spawned.GetComponent<Patrol>();
            if (patrol != null)
                patrol.SetPatrolGroup(patrolGroup);
        }

        SpawnableObject spawnable = spawned.GetComponent<SpawnableObject>();
        spawnable?.RegisterToCulling();
    }

    // 🔹 Permite sortear novamente os prefabs e eventos (por exemplo, a cada noite)
    public void RandomizeSpawns()
    {
        foreach (var entry in spawns)
        {
            entry.chosenData = PickRandomSpawnData(entry.spawnData);
            entry.chosenData.ResolvePoints();
        }
        Debug.Log("[TimerSpawnEvent] Spawns randomizados e resolvidos novamente.");
    }
}

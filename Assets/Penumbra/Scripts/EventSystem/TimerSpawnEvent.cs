using UnityEngine;
using System.Collections.Generic;

public class TimerSpawnEvent : MonoBehaviour
{
    [Header("Configuração de Spawn")]
    public GameObject prefabToSpawn;
    public Transform[] spawnPoints;
    public PatrolPointGroup patrolGroup; // 🔹 Grupo definido no Inspector

    [Header("Timer")]
    public TimerEventReference eventReference;

    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (eventReference == null)
        {
            Debug.LogError($"[TimerSpawnEvent] {name} não possui TimerEventReference configurado!");
            return;
        }

        if (TimerEventScheduler.Instance != null)
        {
            Debug.Log($"[TimerSpawnEvent] Registrando evento para {eventReference.triggerSecond}s.");
            TimerEventScheduler.Instance.AddEvent(eventReference.triggerSecond, TrySpawn);
        }
        else
        {
            Debug.LogError("[TimerSpawnEvent] TimerEventScheduler não encontrado na cena!");
        }
    }

    private void TrySpawn()
    {
        Debug.Log($"[TimerSpawnEvent] Tentando spawnar objeto ({prefabToSpawn?.name ?? "null"})...");

        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("[TimerSpawnEvent] Nenhum spawnPoint configurado!");
            return;
        }

        if (prefabToSpawn == null)
        {
            Debug.LogWarning("[TimerSpawnEvent] Nenhum prefab configurado!");
            return;
        }

        if (player == null)
        {
            Debug.LogWarning("[TimerSpawnEvent] Player não encontrado!");
            return;
        }

        // Filtra apenas spawns em salas onde o jogador NÃO está
        List<Transform> validSpawns = new List<Transform>();

        foreach (var spawn in spawnPoints)
        {
            if (spawn == null)
            {
                Debug.LogWarning("[TimerSpawnEvent] SpawnPoint nulo encontrado, ignorando...");
                continue;
            }

            RoomTracker tracker = spawn.GetComponent<RoomTracker>();
            if (tracker != null && tracker.CurrentRoom != null)
            {
                Room room = tracker.CurrentRoom;

                Debug.Log($"[TimerSpawnEvent] SpawnPoint {spawn.name} está na sala {room.roomName}");

                if (!room.Contains(player.position))
                {
                    Debug.Log($"[TimerSpawnEvent] SpawnPoint {spawn.name} é válido (player fora da sala).");
                    validSpawns.Add(spawn);
                }
                else
                {
                    Debug.Log($"[TimerSpawnEvent] SpawnPoint {spawn.name} ignorado (player dentro da sala).");
                }
            }
            else
            {
                Debug.Log($"[TimerSpawnEvent] SpawnPoint {spawn.name} não possui RoomTracker ou está sem sala associada.");
            }
        }

        if (validSpawns.Count == 0)
        {
            Debug.LogWarning("[TimerSpawnEvent] Nenhum spawn válido encontrado!");
            return;
        }

        // Escolhe um spawn aleatório válido
        Transform chosen = validSpawns[Random.Range(0, validSpawns.Count)];
        Debug.Log($"[TimerSpawnEvent] SpawnPoint escolhido: {chosen.name}");

        GameObject spawned = Instantiate(prefabToSpawn, chosen.position, chosen.rotation);
        Debug.Log($"[TimerSpawnEvent] Objeto {spawned.name} instanciado em {chosen.position}");

        // 🔹 Inicializa o PatrolGroup configurado no Inspector
        if (patrolGroup != null)
        {
            Patrol controller = spawned.GetComponent<Patrol>();
            if (controller != null)
            {
                controller.SetPatrolGroup(patrolGroup);
                Debug.Log($"[TimerSpawnEvent] {spawned.name} vinculado ao PatrolGroup {patrolGroup.name}");
            }
            else
            {
                Debug.LogWarning($"[TimerSpawnEvent] {spawned.name} não possui componente Patrol.");
            }
        }
        else
        {
            Debug.LogWarning("[TimerSpawnEvent] Nenhum PatrolGroup configurado no Inspector!");
        }

        // 🔹 Registra no sistema de culling se aplicável
        SpawnableObject spawnable = spawned.GetComponent<SpawnableObject>();
        if (spawnable != null)
        {
            spawnable.RegisterToCulling();
            Debug.Log($"[TimerSpawnEvent] {spawned.name} registrado no sistema de culling.");
        }
        else
        {
            Debug.Log($"[TimerSpawnEvent] {spawned.name} não possui SpawnableObject, nada a registrar.");
        }
    }
}

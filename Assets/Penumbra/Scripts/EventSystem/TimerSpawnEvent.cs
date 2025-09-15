using UnityEngine;
using System.Collections.Generic;

public class TimerSpawnEvent : MonoBehaviour
{
    [Header("Configura��o de Spawn")]
    public GameObject prefabToSpawn;
    public Transform[] spawnPoints;

    [Header("Timer")]
    public TimerEventReference eventReference; // Agora usamos o TimerEventReference

    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (eventReference == null)
        {
            Debug.LogError($"[TimerSpawnEvent] {name} n�o possui TimerEventReference configurado!");
            return;
        }

        if (TimerEventScheduler.Instance != null)
        {
            Debug.Log($"[TimerSpawnEvent] Registrando evento para {eventReference.triggerSecond}s.");
            TimerEventScheduler.Instance.AddEvent(eventReference.triggerSecond, TrySpawn);
        }
        else
        {
            Debug.LogError("[TimerSpawnEvent] TimerEventScheduler n�o encontrado na cena!");
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
            Debug.LogWarning("[TimerSpawnEvent] Player n�o encontrado!");
            return;
        }

        // Filtra apenas spawns em salas onde o jogador N�O est�
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

                Debug.Log($"[TimerSpawnEvent] SpawnPoint {spawn.name} est� na sala {room.roomName}");

                // Se o jogador n�o est� dentro dessa sala, � um spawn v�lido
                if (!room.Contains(player.position))
                {
                    Debug.Log($"[TimerSpawnEvent] SpawnPoint {spawn.name} � v�lido (player fora da sala).");
                    validSpawns.Add(spawn);
                }
                else
                {
                    Debug.Log($"[TimerSpawnEvent] SpawnPoint {spawn.name} ignorado (player dentro da sala).");
                }
            }
            else
            {
                Debug.Log($"[TimerSpawnEvent] SpawnPoint {spawn.name} n�o possui RoomTracker ou est� sem sala associada.");
            }
        }

        if (validSpawns.Count == 0)
        {
            Debug.LogWarning("[TimerSpawnEvent] Nenhum spawn v�lido encontrado!");
            return;
        }

        // Escolhe um spawn aleat�rio v�lido
        Transform chosen = validSpawns[Random.Range(0, validSpawns.Count)];
        Debug.Log($"[TimerSpawnEvent] SpawnPoint escolhido: {chosen.name}");

        GameObject spawned = Instantiate(prefabToSpawn, chosen.position, chosen.rotation);
        Debug.Log($"[TimerSpawnEvent] Objeto {spawned.name} instanciado em {chosen.position}");

        // Se o prefab possuir SpawnableObject, registra no sistema de culling
        SpawnableObject spawnable = spawned.GetComponent<SpawnableObject>();
        if (spawnable != null)
        {
            spawnable.RegisterToCulling();
            Debug.Log($"[TimerSpawnEvent] {spawned.name} registrado no sistema de culling.");
        }
        else
        {
            Debug.Log($"[TimerSpawnEvent] {spawned.name} n�o possui SpawnableObject, nada a registrar.");
        }
    }
}

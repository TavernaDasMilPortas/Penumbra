using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Versão otimizada de culling usando chunks baseados em salas do RoomTracker.
/// Apenas objetos dentro da sala atual e salas vizinhas são verificados.
/// </summary>
public class ChunkedVisibilityCulling : MonoBehaviour
{
    //[Header("Referências")]
    //public RoomTracker roomTracker; // referência ao RoomTracker do jogador
    //public FacingSystem facingSystem;
    //public Transform player;

    //[Header("Configurações")]
    //public bool disableCompletely = true; // desativa o GameObject ou só o renderer

    //[Header("Layers especiais")]
    //public LayerMask floorLayer;
    //public float floorSafeRadius = 2f;

    //// Armazena os objetos por chunk (Room)
    //private Dictionary<Room, List<Collider>> chunkObjects = new Dictionary<Room, List<Collider>>();

    //// Objetos atualmente ativos
    //private HashSet<Collider> currentlyActive = new HashSet<Collider>();

    //private void Awake()
    //{
    //    if (player == null && Camera.main != null)
    //        player = Camera.main.transform;

    //    if (facingSystem == null)
    //        facingSystem = FindObjectOfType<FacingSystem>();

    //    if (roomTracker == null)
    //        roomTracker = FindObjectOfType<RoomTracker>();

    //    RegisterObjectsByChunks();

    //    // Inicializa todos os objetos como invisíveis
    //    foreach (var kvp in chunkObjects)
    //    {
    //        foreach (var col in kvp.Value)
    //        {
    //            SetObjectActive(col, false);
    //        }
    //    }

    //    // Assina eventos do FacingSystem
    //    facingSystem.OnEnterVision.AddListener(ActivateObject);
    //    facingSystem.OnExitVision.AddListener(DeactivateObject);
    //}

    ///// <summary>
    ///// Pré-cadastra todos os objetos por chunk baseado no RoomTracker
    ///// </summary>
    //private void RegisterObjectsByChunks()
    //{
    //    chunkObjects.Clear();

    //    foreach (var col in FindObjectsOfType<Collider>())
    //    {
    //        Room room = roomTracker.GetRoomAtPosition(col.transform.position);
    //        if (room == null) continue;

    //        if (!chunkObjects.ContainsKey(room))
    //            chunkObjects[room] = new List<Collider>();

    //        chunkObjects[room].Add(col);
    //    }
    //}

    //private void Update()
    //{
    //    Room currentRoom = roomTracker.CurrentRoom;
    //    if (currentRoom == null) return;

    //    // Pega lista de salas a verificar: sala atual + vizinhas
    //    List<Room> roomsToCheck = new List<Room> { currentRoom };
    //    roomsToCheck.AddRange(currentRoom.GetNeighborRooms());

    //    HashSet<Collider> newVisible = new HashSet<Collider>();

    //    foreach (var room in roomsToCheck)
    //    {
    //        if (!chunkObjects.TryGetValue(room, out List<Collider> objectsInRoom)) continue;

    //        foreach (var col in objectsInRoom)
    //        {
    //            if (facingSystem.IsLookingAt(col.bounds.center))
    //            {
    //                newVisible.Add(col);
    //                if (!currentlyActive.Contains(col))
    //                    ActivateObject(col);
    //            }
    //            else
    //            {
    //                if (currentlyActive.Contains(col))
    //                    DeactivateObject(col);
    //            }
    //        }
    //    }
    //}

    //private void ActivateObject(Collider col)
    //{
    //    if (col == null) return;
    //    if (IsFloorAndPlayerOnTop(col)) return;

    //    if (!currentlyActive.Contains(col))
    //    {
    //        SetObjectActive(col, true);
    //        currentlyActive.Add(col);
    //    }
    //}

    //private void DeactivateObject(Collider col)
    //{
    //    if (col == null) return;
    //    if (IsFloorAndPlayerOnTop(col)) return;

    //    if (currentlyActive.Contains(col))
    //    {
    //        SetObjectActive(col, false);
    //        currentlyActive.Remove(col);
    //    }
    //}

    //private void SetObjectActive(Collider col, bool state)
    //{
    //    if (disableCompletely)
    //    {
    //        col.gameObject.SetActive(state);
    //    }
    //    else
    //    {
    //        foreach (Renderer r in col.GetComponentsInChildren<Renderer>(true))
    //            r.enabled = state;
    //    }
    //}

    //private bool IsFloorAndPlayerOnTop(Collider col)
    //{
    //    if (((1 << col.gameObject.layer) & floorLayer) == 0) return false;

    //    Vector3 playerPos = new Vector3(player.position.x, 0f, player.position.z);
    //    Vector3 floorPos = new Vector3(col.transform.position.x, 0f, col.transform.position.z);
    //    return Vector3.Distance(playerPos, floorPos) <= floorSafeRadius;
    //}
}

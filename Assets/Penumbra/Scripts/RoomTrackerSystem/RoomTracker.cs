using UnityEngine;

public class RoomTracker : MonoBehaviour
{
    private Session currentSession;
    private Room currentRoom;

    void Update()
    {
        (Session session, Room room) = SessionManager.Instance.GetLocation(transform.position);

        if (room != currentRoom)
        {
            currentRoom = room;
            currentSession = session;

            if (room != null)
                Debug.Log($"{gameObject.name} entrou na sala {room.roomName} da sessão {session.sessionName}");
            else
                Debug.Log($"{gameObject.name} saiu de todas as salas");
        }
    }
}

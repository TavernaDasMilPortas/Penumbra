using UnityEngine;

public class SessionManager : MonoBehaviour
{
    public static SessionManager Instance;

    public Session[] sessions;

    private void Awake()
    {
        Instance = this;
        sessions = GetComponentsInChildren<Session>();
    }

    public (Session, Room) GetLocation(Vector3 position)
    {
        foreach (var session in sessions)
        {
            Room room = session.GetRoom(position);
            if (room != null)
                return (session, room);
        }
        return (null, null);
    }
}

using UnityEngine;

public class Session : MonoBehaviour
{
    public string sessionName;
    public Room[] rooms;

    private void Awake()
    {
        // pega automaticamente todas as salas filhas
        rooms = GetComponentsInChildren<Room>();
        foreach (var room in rooms)
        {
            room.parentSession = this;
        }
    }

    public Room GetRoom(Vector3 position)
    {
        Room bestRoom = null;
        float closestDist = Mathf.Infinity;

        foreach (var room in rooms)
        {
            if (room.Contains(position))
            {
                // se não tem ainda, pega essa
                if (bestRoom == null)
                {
                    bestRoom = room;
                    closestDist = Vector3.Distance(position, room.GetClosestCenter(position));
                }
                else
                {
                    // compara prioridade
                    if (room.priority > bestRoom.priority)
                    {
                        bestRoom = room;
                        closestDist = Vector3.Distance(position, room.GetClosestCenter(position));
                    }
                    else if (room.priority == bestRoom.priority)
                    {
                        float dist = Vector3.Distance(position, room.GetClosestCenter(position));
                        if (dist < closestDist)
                        {
                            closestDist = dist;
                            bestRoom = room;
                        }
                    }
                }
            }
        }

        return bestRoom;
    }
}

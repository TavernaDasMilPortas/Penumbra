using UnityEngine;

public class Room : MonoBehaviour
{
    public string roomName;
    public Session parentSession;
    public int priority = 0; // para corredores/escadas se precisar

    private Collider[] colliders;

    private void Awake()
    {
        // Pega todos os colliders filhos (inclusive no objeto raiz se tiver)
        colliders = GetComponentsInChildren<Collider>();

        foreach (var col in colliders)
        {
            col.isTrigger = true; // todos viram triggers
        }
    }

    public bool Contains(Vector3 position)
    {
        foreach (var col in colliders)
        {
            if (col.bounds.Contains(position))
                return true;
        }
        return false;
    }

    public Vector3 GetClosestCenter(Vector3 position)
    {
        float closest = Mathf.Infinity;
        Vector3 closestCenter = Vector3.zero;

        foreach (var col in colliders)
        {
            float dist = Vector3.Distance(position, col.bounds.center);
            if (dist < closest)
            {
                closest = dist;
                closestCenter = col.bounds.center;
            }
        }
        return closestCenter;
    }
}

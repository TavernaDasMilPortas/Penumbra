using UnityEngine;
using UnityEngine.AI;

public class DoorLinkGenerator : MonoBehaviour
{
    public Transform leftPoint;
    public Transform rightPoint;
    public bool autoToggle = true;

    private OffMeshLink link;
    private NavMeshObstacle obstacle;

    void Start()
    {
        // Procura NavMeshObstacle no próprio objeto ou em qualquer filho
        obstacle = GetComponentInChildren<NavMeshObstacle>();

        if (obstacle == null)
        {
            Debug.LogWarning("[DoorLinkGenerator] Nenhum NavMeshObstacle encontrado neste objeto ou nos filhos.");
        }

        // cria o link
        link = gameObject.AddComponent<OffMeshLink>();
        link.startTransform = leftPoint;
        link.endTransform = rightPoint;
        link.biDirectional = true;
    }

    public void SetDoorOpen(bool isOpen)
    {
        if (autoToggle)
        {
            link.activated = isOpen;

            if (obstacle != null)
            {
                obstacle.carving = !isOpen;
            }
        }
    }
}

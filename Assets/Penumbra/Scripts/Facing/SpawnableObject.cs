using UnityEngine;

public class SpawnableObject : MonoBehaviour
{
    private Collider col;

    private void Awake()
    {
        col = GetComponent<Collider>();
    }

    private void Start()
    {
        RegisterToCulling();
    }

    public void RegisterToCulling()
    {
        if (VisibilityCulling.Instance != null && col != null)
        {
            VisibilityCulling.Instance.Register(col);
        }
    }
}

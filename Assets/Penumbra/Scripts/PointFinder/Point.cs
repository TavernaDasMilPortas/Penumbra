using UnityEngine;

public class Point : MonoBehaviour
{
    [Header("Identificação (único)")]
    public string objectName;

    [Header("Grupo do Point (opcional)")]
    public PointGroup group;

    [Header("Item a spawnar automaticamente")]
    public Item spawnItem;

    [HideInInspector]
    public Transform selfTransform;

    private void Awake()
    {
        selfTransform = transform;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        selfTransform = transform;
    }
#endif
}

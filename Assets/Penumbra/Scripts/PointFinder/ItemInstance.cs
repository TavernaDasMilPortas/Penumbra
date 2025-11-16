using UnityEngine;

[DisallowMultipleComponent]
public class ItemInstance : MonoBehaviour
{
    public Item data;
    public Point originPoint;
    public int originalLayer;

    private void Awake()
    {
        originalLayer = gameObject.layer;
    }

    public Transform GetOriginTransform()
    {
        return originPoint != null ? originPoint.selfTransform : null;
    }
}

using UnityEngine;

[DisallowMultipleComponent]
public class ItemInstance : MonoBehaviour
{
    [Tooltip("Qual ScriptableObject este objeto representa")]
    public Item data;

    [Tooltip("Point de origem onde o item deve voltar quando desativado")]
    public Point originPoint;

    /// <summary>Retorna o Transform do point associado (seguro mesmo que point seja null).</summary>
    public Transform GetOriginTransform()
    {
        return originPoint != null ? originPoint.selfTransform : null;
    }
}
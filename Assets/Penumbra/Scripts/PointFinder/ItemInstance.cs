using UnityEngine;

[DisallowMultipleComponent]
public class ItemInstance : MonoBehaviour
{
    [Tooltip("Qual ScriptableObject este objeto representa")]
    public Item data;

    [Tooltip("Point de origem onde o item deve voltar quando desativado")]
    public Point originPoint;

    [Tooltip("Layer original do item antes de ir para a mão")]
    public int originalLayer;

    private void Awake()
    {
        // Se ainda não foi configurado, registra automaticamente
        originalLayer = gameObject.layer;
    }

    /// <summary>Retorna o Transform do point associado (seguro mesmo que point seja null).</summary>
    public Transform GetOriginTransform()
    {
        return originPoint != null ? originPoint.selfTransform : null;
    }
}

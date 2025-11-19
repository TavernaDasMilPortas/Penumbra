using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Item", menuName = "Inventário/Item")]
public class Item : ScriptableObject
{
    [Header("Dados base")]
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;
    public bool consumable;

    [Header("Dados de mão/esquerda")]
    public GameObject handPrefab;  // modelo segurado na mão
    public string leftArmState;    // nome do estado no Animator

    [Header("Offsets de transformação")]
    public Vector3 placementOffset = Vector3.zero;
    public Vector3 placementRotationOffset = Vector3.zero;
    public Vector3 placementScaleOffset = Vector3.one;

    public enum ItemAlignmentMode
    {
        None,
        Vertical,       // Alinha Y do alignment point com Y do holder
        Horizontal      // Alinha XZ do alignment point com o holder
    }

    [Header("Alinhamento")]
    public ItemAlignmentMode alignmentMode = ItemAlignmentMode.None;

    [Tooltip("Nome do ponto de alinhamento dentro do prefab")]
    public string alignmentPointName = "AlignmentPoint";

}

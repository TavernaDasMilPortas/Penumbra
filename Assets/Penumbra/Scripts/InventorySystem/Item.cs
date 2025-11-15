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
        None,           // Sem alinhamento especial — usa offsets normais
        Vertical,       // Alinha o Y do AlignmentPoint com o Y do Holder
        Horizontal      // Alinha X/Z do AlignmentPoint com o Holder
    }

    [Header("Alinhamento")]
    public ItemAlignmentMode alignmentMode = ItemAlignmentMode.None;
    public string alignmentPointName = "AlignmentPoint"; // se quiser customizar

}

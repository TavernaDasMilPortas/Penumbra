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
}

using UnityEngine;
[System.Serializable]
[CreateAssetMenu(fileName = "New Item", menuName = "Invent�rio/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public string description;
    public Sprite icon;
    public bool consumable;
}
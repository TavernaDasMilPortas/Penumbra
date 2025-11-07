using UnityEngine;
using static UnityEditor.Progress;

public class CollectableItem : MonoBehaviour, IInteractable
{
    [Header("Item necessário para interação (opcional)")]
    public Item requiredItem;
    public int requiredItemQuantity = 1;

    [TextArea]
    public string interactionMessage = "";

    [Header("Item coletavel ao inteagir")]
    public Item collectedItem;
    public int collectedItemQuantity = 1;

    public Item RequiredItem => requiredItem;
    public int RequiredItemQuantity => requiredItemQuantity;
    public string InteractionMessage => interactionMessage;

    public void Interact()
    {
        if (RequiredItem == null)
        {
            interactionMessage = "Coletou " + collectedItem.itemName;
            PerformInteraction();
        }
        else
        {
            Debug.Log("Item necessário: " + RequiredItem.itemName);
        }
    }

    private void PerformInteraction()
    {
        QuickInventoryManager.Instance.AddItem(collectedItem, collectedItemQuantity);
        Destroy(gameObject);
    }
}
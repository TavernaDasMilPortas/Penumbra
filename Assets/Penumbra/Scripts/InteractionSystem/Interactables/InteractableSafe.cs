using UnityEngine;

public class InteractableSafe : MonoBehaviour, IInteractable
{
    [Header("Item necessário para interação (opcional)")]
    public Item requiredItem;
    public int requiredItemQuantity = 1;

    [TextArea]
    public string interactionMessage = "Interagiu com Interactable";

    [Header("SafeController")]
    public SafeController safeController;
    public Item RequiredItem => requiredItem;
    public int RequiredItemQuantity => requiredItemQuantity;
    public string InteractionMessage => interactionMessage;
    [Header("Flag")]
    public bool isInteractable;
    public bool IsInteractable => isInteractable;

    public void Interact()
    {
        if (RequiredItem == null)
        {
            PerformInteraction();
        }
        else
        {
            Debug.Log("Item necessário: " + RequiredItem.itemName);
        }
    }

    private void PerformInteraction()
    {
        Debug.Log(InteractionMessage);
        safeController.StartInteraction();
    }
}
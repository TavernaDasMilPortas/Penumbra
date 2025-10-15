using UnityEngine;

public class Interactabletest : MonoBehaviour, IInteractable
{
    [Header("Item necessário para interação (opcional)")]
    public Item requiredItem;
    public int requiredItemQuantity = 1;

    [TextArea]
    public string interactionMessage = "Interagiu com Interactable";

    public Item RequiredItem => requiredItem;
    public int RequiredItemQuantity => requiredItemQuantity;
    public string InteractionMessage => interactionMessage;

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
        NightManager.Instance.CompleteTask("interagir com o quadrado");
        // TODO: lógica específica de interação aqui.
    }
}
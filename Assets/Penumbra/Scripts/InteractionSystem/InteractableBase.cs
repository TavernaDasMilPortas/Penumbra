using UnityEngine;

public abstract class InteractableBase : MonoBehaviour, IInteractable
{
    [Header("Configuração de Interação")]
    [SerializeField] private string interactionMessage = "Pressione E para interagir";
    [SerializeField] private bool isInteractable = true;

    [Header("Requisitos (opcional)")]
    [SerializeField] private Item requiredItem;
    [SerializeField] private int requiredItemQuantity = 0;

    // 🔹 Agora todas têm getter e setter públicos, mas ainda aparecem no Inspector
    public string InteractionMessage
    {
        get => interactionMessage;
        set => interactionMessage = value;
    }

    public bool IsInteractable
    {
        get => isInteractable;
        set => isInteractable = value;
    }

    public Item RequiredItem
    {
        get => requiredItem;
        set => requiredItem = value;
    }

    public int RequiredItemQuantity
    {
        get => requiredItemQuantity;
        set => requiredItemQuantity = value;
    }

    public abstract void Interact();
}

using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Collectable : InteractableBase
{
    [Header("Item a ser Coletado")]
    [SerializeField] private Item collectableItem;
    [SerializeField] private int collectableQuantity = 1;

    private void Awake()
    {
        if (collectableItem == null)
            Debug.LogWarning($"[Collectable] {name} não possui um item configurado para coleta.");
    }

    public override void Interact()
    {
        if (!IsInteractable)
        {
            Debug.Log($"[Collectable] {name} não está interagível no momento.");
            return;
        }

        // ✅ Se há item requerido, verifica no inventário
        if (RequiredItem != null)
        {
            if (!QuickInventoryManager.Instance.HasItem(RequiredItem, RequiredItemQuantity))
            {
                Debug.Log($"[Collectable] Você precisa de {RequiredItemQuantity}x {RequiredItem.itemName} para coletar este item.");
                return;
            }
        }

        PerformInteraction();
    }

    private void PerformInteraction()
    {
        if (collectableItem == null)
        {
            Debug.LogWarning($"[Collectable] Nenhum item configurado para coleta em {name}!");
            return;
        }

        QuickInventoryManager.Instance.AddItem(collectableItem, collectableQuantity);
        Debug.Log($"[Collectable] Coletou {collectableQuantity}x {collectableItem.itemName}");

        IsInteractable = false;
        Destroy(gameObject);
    }
}

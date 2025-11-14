using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Collectable : InteractableBase
{
    [Header("Item a ser Coletado")]
    [SerializeField] private Item collectableItem;
    [SerializeField] private int collectableQuantity = 1;

    private ItemInstance itemInstance;

    private void Awake()
    {
        if (collectableItem == null)
            Debug.LogWarning($"[Collectable] {name} não possui um item configurado para coleta.");

        itemInstance = GetComponent<ItemInstance>();
        if (itemInstance == null)
        {
            // se não existir, cria e configura
            itemInstance = gameObject.AddComponent<ItemInstance>();
            itemInstance.data = collectableItem;
        }

        // tenta preservar originPoint se já atribuído
        if (itemInstance.originPoint == null)
        {
            // tenta achar um Point pai próximo
            var p = GetComponentInParent<Point>();
            if (p != null) itemInstance.originPoint = p;
        }
    }

    public override void Interact()
    {
        if (!IsInteractable) return;

        if (RequiredItem != null && !QuickInventoryManager.Instance.HasItem(RequiredItem, RequiredItemQuantity))
        {
            Debug.Log($"[Collectable] Você precisa de {RequiredItemQuantity}x {RequiredItem.itemName} para coletar este item.");
            return;
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

        // adiciona o item no inventário, passando a referência física (este gameObject)
        QuickInventoryManager.Instance.AddItem(collectableItem, collectableQuantity, gameObject);
        Debug.Log($"[Collectable] Coletou {collectableQuantity}x {collectableItem.itemName}");

        // marca não interagível
        IsInteractable = false;

        // parenta no point de origem (se existir) e desativa
        if (itemInstance != null && itemInstance.originPoint != null)
        {
            transform.SetParent(itemInstance.originPoint.selfTransform, worldPositionStays: true);
            transform.position = itemInstance.originPoint.selfTransform.position;
            transform.rotation = itemInstance.originPoint.selfTransform.rotation;
        }

        gameObject.SetActive(false);
    }
}

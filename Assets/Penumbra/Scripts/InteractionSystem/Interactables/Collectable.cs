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
            // cria ItemInstance
            itemInstance = gameObject.AddComponent<ItemInstance>();
            itemInstance.data = collectableItem;
        }

        // 🔥 REGISTRA LAYER ORIGINAL (ESSENCIAL)
        itemInstance.originalLayer = gameObject.layer;

        // tenta preservar originPoint se já existir
        if (itemInstance.originPoint == null)
        {
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

        // adiciona no inventário a referência física
        QuickInventoryManager.Instance.AddItem(collectableItem, collectableQuantity, gameObject);
        Debug.Log($"[Collectable] Coletou {collectableQuantity}x {collectableItem.itemName}");

        IsInteractable = false;

        // envia o objeto para seu origin e o desativa
        if (itemInstance != null && itemInstance.originPoint != null)
        {
            transform.SetParent(itemInstance.originPoint.selfTransform, worldPositionStays: true);
            transform.position = itemInstance.originPoint.selfTransform.position;
            transform.rotation = itemInstance.originPoint.selfTransform.rotation;
        }

        gameObject.SetActive(false);
    }
}

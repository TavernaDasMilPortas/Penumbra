using UnityEngine;

public class ItemHolder : InteractableBase
{
    [Header("Configuração do Holder")]
    [SerializeField] private Transform itemSpawnPoint;
    [SerializeField] public GameObject currentItemObject;
    [SerializeField] public Item currentItem;

    private void Start()
    {
        if (itemSpawnPoint == null)
            itemSpawnPoint = transform;

        if (string.IsNullOrEmpty(InteractionMessage))
            InteractionMessage = "Clique esquerdo para colocar um item.";
    }

    public override void Interact()
    {
        if (!IsInteractable)
        {
            Debug.LogWarning($"[ItemHolder] Interação bloqueada em {gameObject.name}!");
            return;
        }

        if (RequiredItem != null && !QuickInventoryManager.Instance.HasItem(RequiredItem, RequiredItemQuantity))
        {
            ActionHintManager.Instance.ShowHint("E", $"Necessita: {RequiredItem.itemName} x{RequiredItemQuantity}");
            Debug.Log($"[ItemHolder] Item necessário: {RequiredItem.itemName}");
            return;
        }

        if (currentItemObject == null)
            TryPlaceItem();
        else
            TryTakeItem();
    }

    private void TryPlaceItem()
    {
        if (!IsInteractable)
        {
            Debug.LogWarning($"[ItemHolder] Tentou colocar item, mas interação está bloqueada ({gameObject.name}).");
            return;
        }

        Item selectedItem = QuickInventoryManager.Instance.GetSelectedItem();
        if (selectedItem == null)
        {
            Debug.LogWarning("[ItemHolder] Nenhum item selecionado no inventário.");
            return;
        }

        // Remove a instância física do slot selecionado (se houver)
        var instance = QuickInventoryManager.Instance.RemoveInstanceFromSelectedSlot();
        if (instance == null)
        {
            Debug.LogWarning("[ItemHolder] Não há instância física disponível para colocar no holder.");
            return;
        }

        // Parent e posicione na spawn point do holder (posição do pai)
        instance.transform.SetParent(itemSpawnPoint, worldPositionStays: false);

        // força posição igual ao pai (sem contar offsets)
        instance.transform.position = itemSpawnPoint.position;
        instance.transform.rotation = itemSpawnPoint.rotation;

        // aplica offsets do SO
        instance.transform.localPosition = selectedItem.placementOffset;
        instance.transform.localEulerAngles = selectedItem.placementRotationOffset;
        instance.transform.localScale = Vector3.one;

        instance.SetActive(true);

        currentItemObject = instance;
        currentItem = selectedItem;

        // Atualiza mensagem / UI
        InteractionMessage = $"Pressione E para pegar {selectedItem.itemName}";
        InteractionHandler.Instance?.Refresh();

        Debug.Log($"[ItemHolder] Colocou item: {selectedItem.itemName}");
    }

    private void TryTakeItem()
    {
        if (currentItem == null || currentItemObject == null)
        {
            Debug.LogWarning("[ItemHolder] Nenhum currentItem encontrado, operação cancelada.");
            return;
        }

        // Guarda referência temporária
        var instance = currentItemObject;
        var itemSo = currentItem;

        // Limpa holder
        currentItemObject = null;
        currentItem = null;
        InteractionMessage = "Clique esquerdo para colocar um item.";
        InteractionHandler.Instance?.Refresh();

        // Move a instância de volta para seu Point (origin) e desativa
        var instComp = instance.GetComponent<ItemInstance>();
        if (instComp != null && instComp.originPoint != null)
        {
            instance.transform.SetParent(instComp.originPoint.selfTransform, worldPositionStays: false);
            instance.transform.position = instComp.originPoint.selfTransform.position;
            instance.transform.rotation = instComp.originPoint.selfTransform.rotation;
        }
        else
        {
            instance.transform.SetParent(null);
        }

        instance.SetActive(false);

        // Re-adiciona ao inventário (incrementa quantidade e adiciona a instância)
        QuickInventoryManager.Instance.ReturnInstanceToSlot(itemSo, instance);

        Debug.Log($"[ItemHolder] Item retirado e retornado ao inventário: {itemSo.itemName}");
    }

    public void LockHolder(bool state)
    {
        IsInteractable = state;
    }
}

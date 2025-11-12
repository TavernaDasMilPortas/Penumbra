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
        if (IsInteractable == false)
        {
            Debug.LogWarning($"[ItemHolder] Tentou colocar item, mas interação está bloqueada ({gameObject.name}).");
            return;
        }

        Debug.Log("[ItemHolder] Tentando colocar item...");

        Item selectedItem = QuickInventoryManager.Instance.GetSelectedItem();
        if (selectedItem == null)
        {
            Debug.LogWarning("[ItemHolder] Nenhum item selecionado no inventário.");
            return;
        }

        Debug.Log($"[ItemHolder] Item selecionado: {selectedItem.itemName}");

        if (selectedItem.handPrefab != null)
        {
            // Instancia o objeto
            currentItemObject = Instantiate(selectedItem.handPrefab);
            currentItemObject.transform.SetParent(itemSpawnPoint);

            // Tenta encontrar o ponto de alinhamento dentro do prefab
            Transform alignPoint = currentItemObject.transform.Find("AlignmentPoint");
            if (alignPoint != null)
            {
                // Calcula o deslocamento necessário para alinhar o ponto ao spawn
                Vector3 offset = itemSpawnPoint.position - alignPoint.position;
                currentItemObject.transform.position += offset;

                // Igualar rotação do holder
                currentItemObject.transform.rotation = itemSpawnPoint.rotation;
            }
            else
            {
                // Se não tiver ponto de alinhamento, apenas centraliza
                currentItemObject.transform.position = itemSpawnPoint.position;
                currentItemObject.transform.rotation = itemSpawnPoint.rotation;
                Debug.LogWarning($"[ItemHolder] Nenhum 'AlignmentPoint' encontrado em {selectedItem.handPrefab.name}. Usando posição padrão.");
            }

            // Aplica offset manual definido no item (se houver)
            currentItemObject.transform.position += currentItemObject.transform.TransformDirection(selectedItem.placementOffset);

            // Aplica rotação adicional
            currentItemObject.transform.Rotate(selectedItem.placementRotationOffset, Space.Self);

            Debug.Log($"[ItemHolder] Instanciou prefab '{selectedItem.handPrefab.name}' em {itemSpawnPoint.position}");
        }
        else
        {
            Debug.LogWarning($"[ItemHolder] O item '{selectedItem.itemName}' não possui um handPrefab definido.");
        }

        currentItem = selectedItem;
        QuickInventoryManager.Instance.RemoveItem(selectedItem, 1);

        InteractionMessage = $"Pressione E para pegar {selectedItem.itemName}";
        Debug.Log($"[ItemHolder] Colocou item: {selectedItem.itemName}");
    }


    private void TryTakeItem()
    {
        if (currentItem == null)
        {
            Debug.LogWarning("[ItemHolder] Nenhum currentItem encontrado, operação cancelada.");
            return;
        }

        QuickInventoryManager.Instance.AddItem(currentItem, 1);

        if (currentItemObject != null)
            Destroy(currentItemObject);

        currentItemObject = null;
        currentItem = null;

        InteractionMessage = "Clique esquerdo para colocar um item.";
        InteractionHandler.Instance?.Refresh();
    }

    public void LockHolder(bool state)
    {
        IsInteractable = state;
    }
}

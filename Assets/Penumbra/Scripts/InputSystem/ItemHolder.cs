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
        if (!IsInteractable) { Debug.LogWarning($"[ItemHolder] Tentou colocar item, mas interação está bloqueada ({gameObject.name})."); return; }
        Item selectedItem = QuickInventoryManager.Instance.GetSelectedItem(); if (selectedItem == null) { Debug.LogWarning("[ItemHolder] Nenhum item selecionado no inventário."); return; }
        Debug.Log($"[ItemHolder] Item selecionado: {selectedItem.itemName}"); 
        // Instancia o prefab do item
        if (selectedItem.handPrefab != null) 
        { currentItemObject = Instantiate(selectedItem.handPrefab, itemSpawnPoint); 
            currentItemObject.transform.localPosition = Vector3.zero; 
            currentItemObject.transform.localRotation = Quaternion.identity; 
            // Tenta alinhar pelo "AlignmentPoint"
            Transform alignPoint = currentItemObject.transform.Find("AlignmentPoint"); 
            if (alignPoint != null)
            { 
                Vector3 offset = itemSpawnPoint.position - alignPoint.position; 
                currentItemObject.transform.position += offset; 
                currentItemObject.transform.rotation = itemSpawnPoint.rotation; 
            } 
            else
            { 
                Debug.LogWarning($"[ItemHolder] Nenhum 'AlignmentPoint' encontrado em {selectedItem.handPrefab.name}. Usando posição padrão."); 
            } 
            // Offset e rotação definidos pelo item
             currentItemObject.transform.position += currentItemObject.transform.TransformDirection(selectedItem.placementOffset); 
            currentItemObject.transform.Rotate(selectedItem.placementRotationOffset, Space.Self); } 
        else { Debug.LogWarning($"[ItemHolder] O item '{selectedItem.itemName}' não possui um handPrefab definido."); 
        } currentItem = selectedItem; QuickInventoryManager.Instance.RemoveItem(selectedItem, 1); 
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

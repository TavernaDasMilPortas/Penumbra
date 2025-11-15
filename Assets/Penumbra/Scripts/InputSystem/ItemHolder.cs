using UnityEngine;
using static Item;

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
        Item selectedItem = QuickInventoryManager.Instance.GetSelectedItem();
        if (selectedItem == null) return;

        var instance = QuickInventoryManager.Instance.RemoveInstanceFromSelectedSlot();
        if (instance == null) return;

        // ----------------------------
        // 0) RESTAURAR LAYER ORIGINAL
        // ----------------------------
        var instComp = instance.GetComponent<ItemInstance>();
        if (instComp != null)
            SetLayerRecursively(instance, instComp.originalLayer);

        // ----------------------------
        // 1) Parentar SEM manter posição
        // ----------------------------
        instance.transform.SetParent(itemSpawnPoint, worldPositionStays: false);

        // ----------------------------
        // 2) Aplicar rotação e escala DEFINITIVOS
        // ----------------------------
        instance.transform.localRotation = Quaternion.Euler(selectedItem.placementRotationOffset);
        instance.transform.localScale = selectedItem.placementScaleOffset;

        // ----------------------------
        // 3) Resetar posição ao spawn
        // ----------------------------
        instance.transform.localPosition = Vector3.zero;

        // ----------------------------
        // 4) CALCULAR alinhamento após rotação e escala
        // ----------------------------
        Transform alignmentPoint = instance.transform.Find(selectedItem.alignmentPointName);

        if (alignmentPoint != null && selectedItem.alignmentMode != ItemAlignmentMode.None)
        {
            Vector3 worldAP = alignmentPoint.position;
            Vector3 targetPos = itemSpawnPoint.position;

            Vector3 delta = Vector3.zero;

            switch (selectedItem.alignmentMode)
            {
                case ItemAlignmentMode.Vertical:
                    delta = new Vector3(0, targetPos.y - worldAP.y, 0);
                    break;

                case ItemAlignmentMode.Horizontal:
                    delta = new Vector3(targetPos.x - worldAP.x, 0, targetPos.z - worldAP.z);
                    break;
            }

            instance.transform.position += delta;
        }
        else
        {
            instance.transform.localPosition = selectedItem.placementOffset;
        }

        // ----------------------------
        // 5) Ativa instância e conclui
        // ----------------------------
        instance.SetActive(true);
        currentItemObject = instance;
        currentItem = selectedItem;

        InteractionMessage = $"Pressione E para pegar {selectedItem.itemName}";
        InteractionHandler.Instance?.Refresh();
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
            instance.transform.localScale = instComp.data.placementScaleOffset;
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


    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
            SetLayerRecursively(child.gameObject, layer);
    }
}

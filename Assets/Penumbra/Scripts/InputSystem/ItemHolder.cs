using UnityEngine;

public class ItemHolder : MonoBehaviour, IInteractable
{
    [Header("Configuração do Holder")]
    public Transform itemSpawnPoint; // Onde o item físico será colocado
    public GameObject currentItemObject; // Instância física atual
    public Item currentItem; // Dados do item contido

    [TextArea]
    public string interactionMessage = "Pressione E para interagir";

    public Item RequiredItem => null;
    public int RequiredItemQuantity => 0;
    public string InteractionMessage => interactionMessage;

    private void Start()
    {
        if (itemSpawnPoint == null)
            itemSpawnPoint = transform;
    }

    public void Interact()
    {
        Debug.Log($"[ItemHolder] Interact() chamado em {gameObject.name} — currentItem: {(currentItem ? currentItem.itemName : "nenhum")}");

        if (currentItemObject == null)
        {
            TryPlaceItem();
        }
        else
        {
            TryTakeItem();
        }
    }

    private void TryPlaceItem()
    {
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
            currentItemObject.transform.position +=
                currentItemObject.transform.TransformDirection(selectedItem.placementOffset);

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

        interactionMessage = $"Pressione E para pegar {selectedItem.itemName}";
        Debug.Log($"[ItemHolder] Colocou item: {selectedItem.itemName}");
    }


    private void TryTakeItem()
    {
        Debug.Log($"[ItemHolder] Tentando pegar item em {gameObject.name}");

        if (currentItem == null)
        {
            Debug.LogWarning("[ItemHolder] Nenhum currentItem encontrado, operação cancelada.");
            return;
        }

        Debug.Log($"[ItemHolder] Item atual: {currentItem.itemName}");
        Debug.Log($"[ItemHolder] currentItemObject existe? {(currentItemObject != null ? "Sim" : "Não")}");

        // Adiciona o item de volta ao inventário
        QuickInventoryManager.Instance.AddItem(currentItem, 1);
        Debug.Log($"[ItemHolder] Adicionou {currentItem.itemName} de volta ao inventário.");

        if (currentItemObject != null)
        {
            Debug.Log($"[ItemHolder] Destruindo objeto físico '{currentItemObject.name}'");
            DestroyImmediate(currentItemObject);
            Debug.Log("[ItemHolder] Objeto físico destruído.");
        }
        else
        {
            Debug.LogWarning("[ItemHolder] Nenhum objeto físico para destruir (currentItemObject estava null).");
        }

        currentItemObject = null;
        currentItem = null;

        interactionMessage = "Clique esquerdo para colocar um item.";
        Debug.Log("[ItemHolder] Estado limpo, pronto para novo item.");

        // 🔄 Atualiza o handler de interação, se existir
        if (InteractionHandler.Instance != null)
        {
            Debug.Log("[ItemHolder] Atualizando InteractionHandler...");
            InteractionHandler.Instance.Refresh();
        }
        else
        {
            Debug.LogWarning("[ItemHolder] Nenhum InteractionHandler ativo na cena.");
        }
    }
}

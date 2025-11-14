using UnityEngine;

public class LeftArmController : MonoBehaviour
{
    [Header("Socket onde o item será parentado")]
    public Transform itemSocket;

    private GameObject currentItemModel;
    private Item currentItemData;

    public void SetEquippedItem(Item newItem)
    {
        // devolve item atual ao seu origin e desativa
        if (currentItemModel != null)
        {
            var inst = currentItemModel.GetComponent<ItemInstance>();
            if (inst != null)
            {
                Transform origin = inst.GetOriginTransform();
                if (origin != null)
                    currentItemModel.transform.SetParent(origin, worldPositionStays: true);
            }

            currentItemModel.SetActive(false);
            currentItemModel = null;
            currentItemData = null;
        }

        currentItemData = newItem;

        if (newItem == null) return;

        // pega instância física do QuickInventory (não remove do inventário)
        var instObj = QuickInventoryManager.Instance.GetSelectedInstance();
        if (instObj == null)
        {
            Debug.LogWarning($"[LeftArm] Nenhuma instância física disponível para equipar '{newItem.itemName}'.");
            return;
        }

        currentItemModel = instObj;

        // Reparent: parenta e garante posição igual ao pai (world) antes de aplicar offsets
        currentItemModel.transform.SetParent(itemSocket, worldPositionStays: false);

        // force position = parent position (world)
        currentItemModel.transform.position = itemSocket.position;
        currentItemModel.transform.rotation = itemSocket.rotation;

        // aplica offsets do Item (convertendo offsets relativos ao pai)
        currentItemModel.transform.localPosition = newItem.placementOffset;
        currentItemModel.transform.localEulerAngles = newItem.placementRotationOffset;
        currentItemModel.transform.localScale = Vector3.one;

        currentItemModel.SetActive(true);
    }
}

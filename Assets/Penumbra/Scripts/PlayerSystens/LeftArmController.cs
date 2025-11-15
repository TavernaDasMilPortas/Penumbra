using UnityEngine;

public class LeftArmController : MonoBehaviour
{
    [Header("Socket onde o item será parentado")]
    public Transform itemSocket;

    [Header("Layer do item quando está na mão")]
    public LayerMask handLayerMask;

    private int handLayer;
    private GameObject currentItemModel;
    private Item currentItemData;

    private void Awake()
    {
        // converte LayerMask → número da layer
        if (handLayerMask.value != 0)
            handLayer = Mathf.RoundToInt(Mathf.Log(handLayerMask.value, 2));
        else
        {
            handLayer = -1;
            Debug.LogError("[LeftArm] LayerMask da mão não configurado!");
        }
    }

    public void SetEquippedItem(Item newItem)
    {
        // ===============================
        // DEVOLVE ITEM ATUAL AO ORIGIN
        // ===============================

        if (currentItemModel != null)
        {
            var inst = currentItemModel.GetComponent<ItemInstance>();
            if (inst != null)
            {
                Transform origin = inst.GetOriginTransform();
                if (origin != null)
                {
                    currentItemModel.transform.SetParent(origin, worldPositionStays: true);
                    currentItemModel.transform.position = origin.position;
                    currentItemModel.transform.rotation = origin.rotation;
                    currentItemModel.transform.localScale = inst.data.placementScaleOffset;
                }

                // restaura layer original
                SetLayerRecursively(currentItemModel, inst.originalLayer);
            }

            currentItemModel.SetActive(false);
            currentItemModel = null;
            currentItemData = null;
        }

        // ===============================
        // CONFIGURA NOVO ITEM
        // ===============================

        currentItemData = newItem;
        if (newItem == null) return;

        // pega instância física correta do inventário
        var instObj = QuickInventoryManager.Instance.GetSelectedInstance();
        if (instObj == null)
        {
            Debug.LogWarning($"[LeftArm] Nenhuma instância física disponível para equipar '{newItem.itemName}'.");
            return;
        }

        currentItemModel = instObj;

        // ===============================
        // PREPARA PARA EQUIPAR (NÃO ativa ainda)
        // ===============================

        currentItemModel.SetActive(false); // impede artefatos visuais

        // Parent SEM manter posição mundial
        currentItemModel.transform.SetParent(itemSocket, worldPositionStays: false);

        // Força posição = posição do socket
        currentItemModel.transform.position = itemSocket.position;
        currentItemModel.transform.rotation = itemSocket.rotation;

        // Aplica offsets
        currentItemModel.transform.localPosition = newItem.placementOffset;
        currentItemModel.transform.localEulerAngles = newItem.placementRotationOffset;
        currentItemModel.transform.localScale = newItem.placementScaleOffset;

        // Aplica layer da mão
        if (handLayer != -1)
            SetLayerRecursively(currentItemModel, handLayer);

        // ===============================
        // AGORA ATIVA NO FRAME CORRETO
        // ===============================

        currentItemModel.SetActive(true);
    }

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform t in obj.transform)
            SetLayerRecursively(t.gameObject, layer);
    }
}

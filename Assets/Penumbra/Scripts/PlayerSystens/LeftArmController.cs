using UnityEngine;

public class LeftArmController : MonoBehaviour
{
    public Transform itemSocket;
    public LayerMask handLayerMask;

    private int handLayer;
    private GameObject currentItemModel;
    private Item currentItemData;

    private void Awake()
    {
        if (handLayerMask.value != 0)
            handLayer = Mathf.RoundToInt(Mathf.Log(handLayerMask.value, 2));
        else
            handLayer = -1;
    }

    public void SetEquippedItem(Item newItem)
    {
        // ======================================================
        // DEVOLVE ITEM QUE ESTAVA NA MÃO
        //=======================================================
        if (currentItemModel != null)
        {
            // devolve somente se estava parentado ao socket (evita conflito com holders)
            if (currentItemModel.transform.parent == itemSocket)
            {
                var inst = currentItemModel.GetComponent<ItemInstance>();
                if (inst != null)
                {
                    var origin = inst.GetOriginTransform();
                    if (origin != null)
                    {
                        currentItemModel.transform.SetParent(origin, true);
                        currentItemModel.transform.position = origin.position;
                        currentItemModel.transform.rotation = origin.rotation;
                        currentItemModel.transform.localScale = inst.data.placementScaleOffset;

                        SetLayerRecursively(currentItemModel, inst.originalLayer);
                    }
                }

                currentItemModel.SetActive(false);
            }

            currentItemModel = null;
            currentItemData = null;
        }

        // ======================================================
        // NENHUM ITEM PARA EQUIPAR
        //=======================================================
        currentItemData = newItem;
        if (newItem == null)
            return;

        // ======================================================
        // PEGA INSTÂNCIA DO INVENTÁRIO
        //=======================================================
        var newInstance = QuickInventoryManager.Instance.GetSelectedInstance();
        if (newInstance == null)
        {
            Debug.LogWarning($"[LeftArm] Nenhuma instância para equipar {newItem.itemName}");
            return;
        }

        currentItemModel = newInstance;
        currentItemModel.SetActive(false);

        // ======================================================
        // EQUIPA DE FATO
        //=======================================================
        currentItemModel.transform.SetParent(itemSocket, false);

        currentItemModel.transform.localPosition = newItem.placementOffset;
        currentItemModel.transform.localEulerAngles = newItem.placementRotationOffset;
        currentItemModel.transform.localScale = newItem.placementScaleOffset;

        if (handLayer != -1)
            SetLayerRecursively(currentItemModel, handLayer);

        currentItemModel.SetActive(true);
    }

    // chamado pelo holder quando pega o item da mão
    public void NotifyInstanceTakenFromHand(GameObject instance)
    {
        if (instance == null) return;

        if (currentItemModel == instance)
        {
            currentItemModel = null;
            currentItemData = null;

            Debug.Log("[LeftArm] Item retirado da mão pelo holder.");
        }
    }

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;

        foreach (Transform t in obj.transform)
            SetLayerRecursively(t.gameObject, layer);
    }
}

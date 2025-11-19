using UnityEngine;

public class ItemHolder : InteractableBase
{
    public Transform itemSpawnPoint;
    public GameObject currentItemObject;
    public Item currentItem;

    private void Start()
    {
        if (itemSpawnPoint == null)
            itemSpawnPoint = transform;
    }

    public override void Interact()
    {
        if (!IsInteractable) return;

        if (currentItemObject == null)
            TryPlaceItem();
        else
            TryTakeItem();
    }

    private void TryPlaceItem()
    {
        Item selectedItem = QuickInventoryManager.Instance.GetSelectedItem();
        if (selectedItem == null) return;

        GameObject instance = QuickInventoryManager.Instance.RemoveInstanceFromSelectedSlot();
        if (instance == null) return;

        ArmsManager.Instance.leftArm.NotifyInstanceTakenFromHand(instance);

        // define o parent do holder
        instance.transform.SetParent(itemSpawnPoint, false);

        // 🟦 usa o alinhamento unificado
        ItemAlignmentUtility.ApplyAlignment(instance, itemSpawnPoint, selectedItem);

        instance.SetActive(true);

        currentItem = selectedItem;
        currentItemObject = instance;
    }



    private void TryTakeItem()
    {
        var inst = currentItemObject;
        var so = currentItem;

        currentItem = null;
        currentItemObject = null;

        var instComp = inst.GetComponent<ItemInstance>();
        if (instComp != null && instComp.originPoint != null)
        {
            inst.transform.SetParent(instComp.originPoint.selfTransform, false);
            inst.transform.localPosition = Vector3.zero;
            inst.transform.localRotation = Quaternion.identity;
        }
        else inst.transform.SetParent(null);

        inst.SetActive(false);

        QuickInventoryManager.Instance.ReturnInstanceToSlot(so, inst);
    }
}

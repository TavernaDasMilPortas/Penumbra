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
        itemInstance = GetComponent<ItemInstance>();
        if (itemInstance == null)
            itemInstance = gameObject.AddComponent<ItemInstance>();

        itemInstance.data = collectableItem;
        itemInstance.originalLayer = gameObject.layer;
    }

    public override void Interact()
    {
        if (!IsInteractable) return;

        QuickInventoryManager.Instance.AddItem(collectableItem, collectableQuantity, gameObject);

        IsInteractable = false;
        gameObject.SetActive(false);
    }
}
